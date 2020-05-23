using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CMSController;
using Mvvm.Async;

namespace CMS_Application.ViewModel
{
    public class DataStatusViewModel : IDataStateViewModel, IDisposable
    {
        private CancellationTokenSource _cancellator;

        private readonly Controller _controller;

        private DataStatus? _dataStatus;
        private int _updatingProgress;
        private int _entitiesCount;
        private string _timeLeft;

        private DateTime _started;

        public DataStatusViewModel(Controller controller)
        {
            _controller = controller;

            _controller.DataStatusChanged += OnDataStatusChanged;
            _controller.NewDataDownloaded += OnNewEntitiesDownaloded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DataStatus DataStatus
        {
            get
            {
                if (_dataStatus == null)
                    _dataStatus = _controller.DataStatus;

                return _dataStatus.Value;
            }

            set
            {
                if (_dataStatus != value)
                {
                    _dataStatus = value;
                    OnPropertyChanged();
                    OnPropertyChanged("IsSyncing");
                    OnPropertyChanged("IsSyncingStatus");
                    OnPropertyChanged("IsSyncingData");
                    OnPropertyChanged("IsUnknownStatus");
                    OnPropertyChanged("IsNeedToUpdate");
                    OnPropertyChanged("IsSynced");
                    OnPropertyChanged("IsConnectionError");
                    OnPropertyChanged("IsNotNeedToUpdate");
                    OnPropertyChanged("IsNeedUpdateStatus");
                }
            }
        }

        public bool IsSyncing => (DataStatus == DataStatus.DataUpdating) || (DataStatus == DataStatus.StatusUpdating);

        public bool IsSynced => DataStatus == DataStatus.Synced;

        public bool IsNeedUpdateStatus => !IsSyncing && !IsNeedToUpdate;

        public bool IsSyncingData => DataStatus == DataStatus.DataUpdating;

        public bool IsSyncingStatus => DataStatus == DataStatus.StatusUpdating;

        public bool IsUnknownStatus => DataStatus == DataStatus.UnknownStatus;

        public bool IsNeedToUpdate => DataStatus == DataStatus.NeedToUpdate;
        public bool IsNotNeedToUpdate => DataStatus != DataStatus.NeedToUpdate;

        public bool IsConnectionError => DataStatus == DataStatus.ConnectionError;

        public int UpdatingProgress
        {
            get { return _updatingProgress; }
            set
            {
                if (_updatingProgress != value)
                {
                    _updatingProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                if (_timeLeft != value)
                {
                    _timeLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        public int EntitiesInDatabase
        {
            get
            {
                _entitiesCount = _controller.EntitiesInDatabase;
                return _entitiesCount;
            }

            set
            {
                if (_entitiesCount != value)
                {
                    OnPropertyChanged();
                }
            }
        }

        public ICommand UpdateStatusCommand
        {
            get
            {
                return new AsyncCommand(
                    async () =>
                    {
                        _cancellator = new CancellationTokenSource();
                        await _controller.SyncStatusAsync(_cancellator.Token);
                    },
                    () => !_controller.IsSyncing);
            }
        }

        public ICommand SyncDataCommand
        {
            get
            {
                return new AsyncCommand(
                    async () =>
                    {
                       _started = DateTime.Now;

                       _cancellator = new CancellationTokenSource();
                       var progress = new Progress<double>(OnDownloadingProgress);
                        
                       await _controller.SyncDataAsync(progress, _cancellator.Token);
                    },
                    () => !_controller.IsSyncing);
            }
        }

        public ICommand CancelSyncCommand
        {
            get
            {
                return new AsyncCommand(
                    async () => await Task.Run(() => _cancellator.Cancel()),
                    () => true);
            }
        }
        
        void OnDownloadingProgress(double progress)
        {
            if (progress > 0.001)
            {
                TimeSpan duration = DateTime.Now - _started;
                
                TimeLeft = TimeSpan.FromSeconds(duration.Seconds / progress * 100).ToString(@"dd\.hh\:mm\:ss");
            }

            UpdatingProgress = (int)progress;
            OnPropertyChanged("EntitiesInDatabase");
        }

        void OnDataStatusChanged(DataStatus dataStatus)
        {
            DataStatus = dataStatus;
        }

        void OnNewEntitiesDownaloded()
        {
            EntitiesInDatabase = _controller.EntitiesInDatabase;
        }

        public void Dispose()
        {
            _controller.DataStatusChanged -= OnDataStatusChanged;
            _controller.NewDataDownloaded -= OnNewEntitiesDownaloded;
        }
    }
}
