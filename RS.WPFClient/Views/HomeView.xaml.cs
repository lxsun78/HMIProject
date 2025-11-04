using Microsoft.Extensions.DependencyInjection;
using RS.Commons.Attributs;
using RS.Widgets.Controls;


namespace RS.WPFClient.Client.Views
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
