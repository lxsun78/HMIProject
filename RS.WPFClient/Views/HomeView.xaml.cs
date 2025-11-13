using Microsoft.Extensions.DependencyInjection;
using RS.Commons.Attributs;
using RS.Commons.Helper;
using RS.Widgets.Controls;
using RS.WPFClient.IServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;


namespace RS.WPFClient.Views
{
    [ServiceInjectConfig(ServiceLifetime.Singleton)]
    public partial class HomeView : RSWindow
    {
        public HomeView()
        {
            InitializeComponent();
        }
    }
}
