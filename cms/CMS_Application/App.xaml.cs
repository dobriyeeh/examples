using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CMS_Application.ViewModel;
using CMSController;

namespace CMS_Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Controller _controller;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                _controller = new Controller();

                //_controller.FixDatabase(FixedIssues.MissedIds);

                var mainView = new MainWindowView(_controller);
                mainView.Show();
                mainView.DataContext = new DataStatusViewModel(_controller);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            base.OnStartup(e);
        }
    }
}
