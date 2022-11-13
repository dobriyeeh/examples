using System.ComponentModel;
using System.Windows.Input;
using CMSController;

namespace CMS_Application.ViewModel
{
    public interface IDataStateViewModel : INotifyPropertyChanged
    {
        DataStatus DataStatus { get; }

        int UpdatingProgress { get; }

        int EntitiesInDatabase { get; }

        ICommand UpdateStatusCommand { get; }
    }
}