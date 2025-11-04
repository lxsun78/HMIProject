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
        void Show<TViewModel, TView>()
          where TViewModel : class
          where TView : Window;

        bool? ShowDialog<TViewModel, TView>(TViewModel viewModel)
          where TViewModel : class
          where TView : Window;

        void Close<TViewModel>(TViewModel viewModel)
           where TViewModel : class;
    }
}
