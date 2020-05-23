using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using DataModel;
using Mvvm.Async;
using CMSController;
using Microsoft.Win32;

namespace CMS_Application.ViewModel
{
    public class EntitiesQueryViewModel : IEntitiesQueryViewModel
    {
        private Controller _controller;

        private CancellationTokenSource _cancellator;

        private IEnumerable<EntityInformation> _entities = Enumerable.Empty<EntityInformation>();
        private string _keyword = "";
        private string _filePath;

        private bool _searchByMonthYear = true;
        private DateTime _monthYearFilter = new DateTime(2020, 1, 1);

        private DataStatus? _dataStatus;

        public EntitiesQueryViewModel(Controller controller)
        {
            _controller = controller;

            _controller.DataStatusChanged += OnDataStatusChanged;
        }

        public string Keyword
        {
            get { return _keyword; }

            set
            {
                _keyword = value;
                OnPropertyChanged();
            }
        }

        public bool SearchByMonthYear
        {
            get { return _searchByMonthYear; }

            set
            {
                _searchByMonthYear = value;
                OnPropertyChanged();
            }
        }

        public DateTime MonthYearFilter
        {
            get { return _monthYearFilter; }

            set
            {
                _monthYearFilter = value;
                OnPropertyChanged();
            }
        }

        public bool IsSyncing => (DataStatus == DataStatus.DataUpdating) || (DataStatus == DataStatus.StatusUpdating);

        public bool IsNotSyncing => !IsSyncing;
        
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
                    OnPropertyChanged("IsNotSyncing");
                }
            }
        }

        public IEnumerable<EntityInformation> Entities
        {
            get { return _entities; }
            set
            {
                if (!ReferenceEquals(_entities, value))
                {
                    _entities = value;
                    OnPropertyChanged();
                    OnPropertyChanged("EntitiesCount");
                }
            }
        }

        public int EntitiesCount => _entities.Count();

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        public ICommand QueryCommand
        {
            get
            {
                return new AsyncCommand(
                    async () =>
                    {
                        try
                        {
                            _cancellator = new CancellationTokenSource();

                            Entities = await _controller.QueryEntities(Keyword, SearchByMonthYear, MonthYearFilter, _cancellator.Token);

                            if (!string.IsNullOrEmpty(FilePath))
                                await _controller.SaveEntities(Entities, FilePath);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString(), "Updating error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    },
                    () => !_controller.IsSyncing);
            }
        }

        public ICommand GetFilePath
        {
            get
            {
                return new AsyncCommand(
                    async () =>
                    {
                        var fileDialog = new SaveFileDialog();
                        fileDialog.OverwritePrompt = false;
                        fileDialog.DefaultExt = ".text";
                        fileDialog.Filter = "Text documents |*.txt";
                        if (fileDialog.ShowDialog() == true)
                        {
                            FilePath = fileDialog.FileName;
                        }
                    },
                    () => true);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnDataStatusChanged(DataStatus dataStatus)
        {
            DataStatus = dataStatus;
        }
    }
}