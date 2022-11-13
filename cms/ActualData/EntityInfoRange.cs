using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel;

namespace ActualData
{
    public class EntityInfoRange : IEntityInfoRange
    {
        private const long predefindedFirst2016CorporateEntity = 3859026;
        private const long predefindedFirst2017CorporateEntity = 3977030;
        private const long predefindedFirst2020CorporateEntity = 4546850;

        public const long StartFromYear = 2020;

        private static DateTime _lastSearchTime;
        private static long _cachedLastEntityNumber = -1;

        public static TimeSpan CacheExpirationPeriod => TimeSpan.FromSeconds(30);

        private static long GetCachedLastEntityNumber()
        {
            if (_cachedLastEntityNumber == -1)
                return -1;

            if ((DateTime.Now - _lastSearchTime) > CacheExpirationPeriod)
            {
                _cachedLastEntityNumber = -1;
            }

            return _cachedLastEntityNumber;
        }

        public static void ResetCache()
        {
            _lastSearchTime = default(DateTime);
            _cachedLastEntityNumber = -1;
        }

        public async Task<long> FindLastEntityNumberAsync(long startSearchFrom = -1, CancellationToken? cancellationToken = null)
        {
            long result = GetCachedLastEntityNumber();
            if (result != -1)
                return result;

            if (cancellationToken?.IsCancellationRequested == true)
                return -1;

            long possibleResult;
            long currSearchFrom = startSearchFrom;

            var downloader = new DataDownloader();

            bool flag;

            do
            {
                flag = false;

                possibleResult = await TryFindLastEntityNumberAsync(currSearchFrom, cancellationToken);
                if (possibleResult == -1)
                    return possibleResult;

                // MOVE TO SEARCH RULE
                for (int i = 0; i < 12; i++)
                {
                    int possibleShift = new Random().Next(2 << i) + 1;

                    long possibleIndex = possibleResult + possibleShift;
                    var nextEntity = await downloader.DownloadAsync(possibleIndex, false, cancellationToken);

                    if ((nextEntity?.Status == DownloadStatus.Ok) && (nextEntity.Entity != null))
                    {
                        currSearchFrom = possibleIndex;
                        flag = true;
                        break;
                    }
                }
            }
            while (flag);

            _cachedLastEntityNumber = possibleResult;
            _lastSearchTime = DateTime.Now;

            return possibleResult;
        }

        private async Task<long> TryFindLastEntityNumberAsync(long startSearchFrom = -1, CancellationToken? cancellationToken = null)
        {
            long result = startSearchFrom;

            var downloader = new DataDownloader();

            const int maxGarantTries = 1024;
            int garant = maxGarantTries + 1;
            long currentIndex = startSearchFrom > 0 ? startSearchFrom : await FindFirstEntityNumberByYearAsync(StartFromYear);
            long currentStep = 1024;

            var cache = new Dictionary<long, DownloadResult>();

            while (--garant > 0)
            {
                if (cancellationToken?.IsCancellationRequested == true)
                    return -1;

                bool firstTime = garant == maxGarantTries;

                long nextIndex = !firstTime ? currentIndex + currentStep : currentIndex;

                DownloadResult nextEntity;
                DownloadResult nextEntityPlus1;

                if (!cache.TryGetValue(nextIndex, out nextEntity))
                {
                    nextEntity = await downloader.DownloadAsync(nextIndex, false, cancellationToken);

                    if (cancellationToken?.IsCancellationRequested == true)
                        return -1;
                }

                if ((nextEntity == null) || (nextEntity.Status == DownloadStatus.NetworkProblem))
                    throw new NetworkException("Network issue");

                cache[nextIndex] = nextEntity;

                if (nextEntity.Status == DownloadStatus.EntityIsNotExists)
                {
                    currentStep = currentStep > 1 ? currentStep / 2 : 1;
                    continue;
                }

                if (!cache.TryGetValue(nextIndex + 1, out nextEntityPlus1))
                {
                    nextEntityPlus1 = await downloader.DownloadAsync(nextIndex + 1, false, cancellationToken);

                    if (cancellationToken?.IsCancellationRequested == true)
                        return -1;

                    cache[nextIndex + 1] = nextEntity;
                }

                if ((nextEntityPlus1 == null) || (nextEntityPlus1.Status == DownloadStatus.NetworkProblem))
                {
                    throw new NetworkException("Network issue");
                }

                if (nextEntityPlus1.Status == DownloadStatus.EntityIsNotExists)
                {
                    result = nextIndex;
                    break;
                }

                if (!firstTime)
                {
                    currentIndex = nextIndex;
                    currentStep *= 2;
                }
            }

            return result;
        }

        public Task<long> FindFirstEntityNumberByYearAsync(long year = -1, CancellationToken? cancellationToken = null)
        {
            switch (year)
            {
                case 2016:
                    return Task.FromResult(predefindedFirst2016CorporateEntity);

                case 2017:
                    return Task.FromResult(predefindedFirst2017CorporateEntity);

                case 2020:
                    return Task.FromResult(predefindedFirst2020CorporateEntity);

                default:
                    return Task.FromResult(predefindedFirst2020CorporateEntity);
            }
        }
    }
}