using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActualData;
using DataModel;

namespace CMSController
{
    public class Controller : IDisposable
    {
        internal EntitiesContext EntitiesContext { get; }

        private readonly object _statusLock = new object();

        private DataStatus _dataStatus = DataStatus.UnknownStatus;

        public DataStatus DataStatus
        {
            get { return _dataStatus; }
            set
            {
                if (_dataStatus != value)
                {
                    _dataStatus = value;
                    OnDataStatusChanged(_dataStatus);
                }
            }
        }

        public event DataStatusHandler DataStatusChanged;

        public event Action NewDataDownloaded;

        public bool IsSyncing => (DataStatus == DataStatus.DataUpdating) || (DataStatus == DataStatus.StatusUpdating);

        public int EntitiesInDatabase => EntitiesContext.Entities.Count();

        //public IEnumerable<string> QueriedKeywords => EntitiesContext.Keywords.Select(val => val.Value);

        public TestUtils TestUtils => new TestUtils(this);

        private string KeywordFilePath
        {
            get
            {
                string directory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                return directory + "\\keywords.txt";
            }
        }

        public void AddNewKerword(string keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword?.Trim()))
                    return;

                string directory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                string path = directory + "\\keywords.txt";

                if (!File.Exists(path))
                    File.Create(path).Close();

                var keywords = new List<string>(File.ReadAllLines(KeywordFilePath));
                if (!keywords.Contains(keyword))
                {
                    keywords.Add(keyword);

                    File.WriteAllLines(path, keywords);
                }
            }
            catch (Exception)
            {
            }
        }

        public Controller()
        {
            DataStatus = DataStatus.UnknownStatus;

            EntitiesContext = new EntitiesContext();
        }

        public async Task<IList<EntityInformation>> GetListOfAllEntitiesAsync(CancellationToken? cancelToken = null)
        {
            return await EntitiesContext.Entities.ToListAsync(cancelToken ?? default(CancellationToken));
        }

        public async Task<EntityInformation> GetEntityInformation(long entityNumber, CancellationToken? cancelToken = null)
        {
            return await EntitiesContext.Entities.FirstOrDefaultAsync(entity => entity.EntityNumber == entityNumber, cancelToken ?? default(CancellationToken));
        }

        public async Task<IList<EntityInformation>> QueryEntities(string keyword, bool searchByMonthYear, DateTime? monthYearFilter, CancellationToken? cancelToken = null)
        {
            AddNewKerword(keyword);

            const string californiaState = "CALIFORNIA";

            var result = new List<EntityInformation>();

            string queryString = keyword?.Trim();
            if (string.IsNullOrEmpty(queryString))
                return result;

            Action<EntityInformation> addToResult = entity =>
            {
                if ((entity?.EntityName.IndexOf(queryString, StringComparison.OrdinalIgnoreCase) != -1) &&
                    (entity?.Jurisdiction.IndexOf(californiaState, StringComparison.OrdinalIgnoreCase) != -1))
                {
                    if (!searchByMonthYear ||
                        ((entity?.RegistrationDate.Month == monthYearFilter?.Month) && (entity?.RegistrationDate.Year == monthYearFilter?.Year)))
                    {
                        result.Add(entity);
                    }
                }
            };

            await EntitiesContext.Entities.ForEachAsync(addToResult, cancelToken ?? default(CancellationToken));

            result.Sort((ent1, ent2) => string.Compare(ent1.EntityName, ent2.EntityName, StringComparison.OrdinalIgnoreCase));

            return result;
        }

        public async Task SaveEntities(IEnumerable<EntityInformation> entities, string filePath)
        {
            var exportEntities = new ExportEntities();
            await exportEntities.ToFile(filePath, entities);
        }


        public async Task<bool> SyncStatusAsync(CancellationToken? cancelToken = null)
        {
            lock (_statusLock)
            {
                if (IsSyncing)
                    return false;

                DataStatus = DataStatus.StatusUpdating;
            }

            try
            {
                var entityInfoRange = new EntityInfoRange();
                long lastKnownEntityNumber = await GetLastKnownEntityNumberAsync(cancelToken);
                long lastEntityNumberOnWebSite = await entityInfoRange.FindLastEntityNumberAsync(lastKnownEntityNumber, cancelToken);

                if (lastEntityNumberOnWebSite <= 0)
                {
                    DataStatus = DataStatus.UnknownStatus;
                    return false;
                }

                DataStatus = lastKnownEntityNumber == lastEntityNumberOnWebSite ? DataStatus.Synced : DataStatus.NeedToUpdate;
            }
            catch (Exception ex)
            {
                if (ex is NetworkException)
                    DataStatus = DataStatus.ConnectionError;
                else
                    DataStatus = DataStatus.UnknownStatus;
            }

            return DataStatus != DataStatus.UnknownStatus;
        }

        public async Task<bool> SyncDataAsync(IProgress<double> progress = null, CancellationToken? cancelToken = null)
        {
            lock (_statusLock)
            {
                if (IsSyncing)
                    return false;

                DataStatus = DataStatus.DataUpdating;
            }

            try
            {
                progress?.Report(0);

                var entityInfoRange = new EntityInfoRange();

                long lastKnownEntityNumber = await GetLastKnownEntityNumberAsync(cancelToken);
                progress?.Report(0.09);

                long lastEntityNumberOnWebSite = await entityInfoRange.FindLastEntityNumberAsync(lastKnownEntityNumber, cancelToken);

                progress?.Report(0.1);

                if (lastEntityNumberOnWebSite <= 0)
                {
                    return false;
                }
                if (lastEntityNumberOnWebSite == lastKnownEntityNumber)
                {
                    DataStatus = DataStatus.Synced;
                    return true;
                }

                double progressValue = 0.0;
                double progressStep = 100.0 / (lastEntityNumberOnWebSite - lastKnownEntityNumber);

                for (long currNumber = lastKnownEntityNumber + 1; currNumber <= lastEntityNumberOnWebSite; currNumber++)
                {
                    if (cancelToken?.IsCancellationRequested == true)
                        return false;

                    int counter = 3600;
                    do
                    {
                        try
                        {
                            await UpdateEntityAsync(currNumber, cancelToken);
                            break;
                        }
                        catch (NetworkException ex)
                        {
                            Debug.WriteLine(ex.Message);
                            await Task.Delay(1000);
                        }
                    }
                    while (--counter > 0);

                    progressValue += progressStep;
                    progress?.Report(progressValue);
                }

                DataStatus = DataStatus.Synced;

                OnNewDataDownloaded();
            }
            catch (Exception ex)
            {
                if (ex is NetworkException)
                    DataStatus = DataStatus.ConnectionError;
                else
                    DataStatus = DataStatus.UnknownStatus;
            }
            finally
            {
                if (DataStatus != DataStatus.Synced)
                {
                    progress?.Report(100.0);
                    DataStatus = DataStatus.NeedToUpdate;
                }
            }

            return true;
        }

        internal async Task<long> GetLastKnownEntityNumberAsync(CancellationToken? cancelToken = null)
        {
            var token = cancelToken ?? default(CancellationToken);
            if (!await EntitiesContext.Entities.AnyAsync(token))
            {
                var range = new EntityInfoRange();
                return await range.FindFirstEntityNumberByYearAsync();
            }

            return await EntitiesContext.Entities.MaxAsync(entity => entity.EntityNumber, token);
        }

        internal async Task<bool> UpdateEntityAsync(long entityNumber, CancellationToken? cancelToken = null)
        {
            var downloader = new DataDownloader();
            var downloadEntity = await downloader.DownloadAsync(entityNumber, true, cancelToken);
            switch (downloadEntity.Status)
            {
                case DownloadStatus.EntityIsNotExists:
                case DownloadStatus.Canceled:
                    return false;

                case DownloadStatus.NetworkProblem:
                    throw new NetworkException("Can't update entity");
            }

            if (downloadEntity.Entity == null)
                return false;

            CancellationToken token = cancelToken ?? default(CancellationToken);

            var entity = await EntitiesContext.Entities.FirstOrDefaultAsync(currEntity => currEntity.EntityNumber == entityNumber, token);
            if (entity == default(EntityInformation))
            {
                EntitiesContext.Entities.Add(downloadEntity.Entity);
            }
            else
            {
                entity.CopyFrom(downloadEntity.Entity, false);
            }

            await EntitiesContext.SaveChangesAsync(token);
            return true;
        }

        public void Dispose()
        {
            EntitiesContext.Dispose();
        }

        protected virtual void OnDataStatusChanged(DataStatus datastatus)
        {
            DataStatusChanged?.Invoke(datastatus);
        }

        protected virtual void OnNewDataDownloaded()
        {
            NewDataDownloaded?.Invoke();
        }
    }
}
