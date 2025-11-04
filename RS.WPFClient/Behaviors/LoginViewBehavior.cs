using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xaml.Behaviors;
using RS.Commons;
using RS.WPFClient.Client.Controls;
using RS.WPFClient.Client.IServices;
using RS.WPFClient.Client.Views;
using RS.Widgets.Behaviors;
using RS.Widgets.Controls;
using System.Windows;

namespace RS.WPFClient.Client.Behaviors
{
    public class LoginViewBehavior : Behavior<LoginView>, ILoginViewService
    {

        public ILoginViewService ServiceProvider
        {
            get { return (ILoginViewService)GetValue(ServiceProvidereProperty); }
            set { SetValue(ServiceProvidereProperty, value); }
        }

        public static readonly DependencyProperty ServiceProvidereProperty =
            DependencyProperty.Register("ServiceProvider", typeof(ILoginViewService), typeof(LoginViewBehavior), new PropertyMetadata(null));



        public async Task<OperateResult> CloseAsync()
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                this.AssociatedObject.Close();
            });

            return OperateResult.CreateSuccessResult();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.ServiceProvider = this;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
