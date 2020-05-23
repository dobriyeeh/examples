using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CMSController;
using CMS_Application.View;
using CMS_Application.ViewModel;

namespace CMS_Application
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        private Controller _controller;

        public MainWindowView(Controller controller)
        {
            _controller = controller;

            InitializeComponent();

            var syncDataControl = new SyncDataControl();
            syncDataControl.DataContext = new DataStatusViewModel(_controller);
            SyncDataWrapper.Children.Add(syncDataControl);

            var entitiesQueryView = new EntitiesQueryView();
            entitiesQueryView.DataContext = new EntitiesQueryViewModel(_controller);
            QueryWrapper.Children.Add(entitiesQueryView);
        }
    }
}
