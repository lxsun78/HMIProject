using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RS.WPFClient.Client.Models
{
    public class MenuItemViewModel
    {
        public string Header { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public ObservableCollection<MenuItemViewModel> Children { get; set; }
    }
}
