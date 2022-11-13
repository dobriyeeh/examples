using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;
using Path = System.IO.Path;

namespace CMS_Application.View
{
    /// <summary>
    /// Interaction logic for EntitiesQueryView.xaml
    /// </summary>
    public partial class EntitiesQueryView : UserControl
    {
        public EntitiesQueryView()
        {
            InitializeComponent();
        }

        private string KeywordFilePath
        {
            get
            {
                string directory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                return directory + "\\keywords.txt";
            }
        }

        private void Url_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string directory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                string path = directory + "\\keywords.txt";

                if (!File.Exists(path))
                    File.Create(path).Close();

                Process.Start(path);
            }
            catch (Exception)
            {
            }
        }

        private void ComboBoxKeyword_GotFocus(object sender, RoutedEventArgs e)
        {
            ComboBoxKeyword.Items.Clear();

            try
            {
                if (File.Exists(KeywordFilePath))
                {
                    var keywords = new List<string>(File.ReadAllLines(KeywordFilePath));
                    keywords.Sort();

                    foreach (var currKeyword in keywords)
                    {
                        ComboBoxKeyword.Items.Add(currKeyword);
                    }
                }
            }
            catch (IOException)
            {
            }
        }
    }
}
