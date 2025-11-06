using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RS.Widgets.Interfaces
{
    public interface IWindowService
    {
        void ShowAsync<TViewModel, TView>()
          where TViewModel : class
          where TView : Window;

        bool? ShowDialogAsync<TViewModel, TView>(TViewModel viewModel)
          where TViewModel : class
          where TView : Window;

        void CloseAsync<TViewModel>(TViewModel viewModel)
           where TViewModel : class;
    }
}
