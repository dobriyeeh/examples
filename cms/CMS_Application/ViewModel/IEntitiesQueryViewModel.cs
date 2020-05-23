using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMSController;
using DataModel;

namespace CMS_Application.ViewModel
{   
    public interface IEntitiesQueryViewModel : INotifyPropertyChanged
    {   
        string Keyword { get; set; }

        IEnumerable<EntityInformation> Entities { get; }

        ICommand QueryCommand { get; }

        DataStatus DataStatus { get; set;  }

        string FilePath { get; set; }

        int EntitiesCount { get; }

        bool SearchByMonthYear { get; set; }
    }
}
