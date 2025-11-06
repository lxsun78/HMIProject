using Microsoft.Extensions.DependencyInjection;
using RS.Commons.Attributs;
using RS.Widgets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RS.Widgets.Services
{
    [ServiceInjectConfig(ServiceLifetime.Singleton, ServiceType = typeof(IWindowService))]
    public class WindowService : IWindowService
    {
        private readonly IServiceProvider ServiceProvider;

        private readonly Dictionary<Type, Type> ViewMappings = new();

        private readonly Dictionary<object, Window> ViewModelInstances = new();

        public WindowService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        private void RegisterMapping<TViewModel, TView>()
            where TViewModel : class
            where TView : Window
        {
            var viewModelType = typeof(TViewModel);
            if (!ViewMappings.ContainsKey(viewModelType))
            {
                ViewMappings[viewModelType] = typeof(TView);
            }
        }

        public void ShowAsync<TViewModel, TView>()
          where TViewModel : class
          where TView : Window
        {
            var viewModel = ServiceProvider.GetRequiredService<TViewModel>();
            RegisterMapping<TViewModel, TView>();
            var window = CreateWindow(viewModel);
            ViewModelInstances[viewModel] = window;
            Application.Current.Dispatcher.Invoke(() =>
            {
                window.Show();
            });
        }

        public bool? ShowDialogAsync<TViewModel, TView>(TViewModel viewModel)
            where TViewModel : class
            where TView : Window
        {
            RegisterMapping<TViewModel, TView>();
            var window = CreateWindow(viewModel);
            ViewModelInstances[viewModel] = window;

            return Application.Current.Dispatcher.Invoke(() =>
              {
                  return window.ShowDialog();
              });
        }

        public void CloseAsync<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            if (ViewModelInstances.TryGetValue(viewModel, out var window))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    window.Close();
                });

                ViewModelInstances.Remove(viewModel);
            }
        }
        private Window CreateWindow<TViewModel>(TViewModel viewModel) where TViewModel : class
        {
            var viewModelType = typeof(TViewModel);
            if (!ViewMappings.TryGetValue(viewModelType, out var viewType))
            {
                throw new KeyNotFoundException(viewModelType.Name);
            }

            var window = ServiceProvider.GetRequiredService(viewType) as Window;
            if (window == null)
            {
                throw new KeyNotFoundException(viewType.Name);
            }
            window.DataContext = viewModel;
            return window;
        }


    }
}
