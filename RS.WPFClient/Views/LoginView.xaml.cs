using Microsoft.Extensions.DependencyInjection;
using RS.Commons.Attributs;
using RS.Widgets.Controls;

namespace RS.WPFClient.Views
{
    [ServiceInjectConfig(ServiceLifetime.Singleton)]
    public partial class LoginView : RSWindow
    {
        public LoginView()
        {
            InitializeComponent();
        }
    }
}
