using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xaml.Behaviors;
using RS.WPFClient.Client.IServices;
using RS.Widgets.Behaviors;
using RS.Widgets.Controls;
using RS.Widgets.Models;
using System.Windows;

namespace RS.WPFClient.Client.Behaviors
{
    public class QRCodeLoginBehavior : Behavior<RSQRCodeLogin>, IQRCodeLoginService
    {

        public IQRCodeLoginService ServiceProvider
        {
            get { return (IQRCodeLoginService)GetValue(ServiceProvidereProperty); }
            set { SetValue(ServiceProvidereProperty, value); }
        }

        public static readonly DependencyProperty ServiceProvidereProperty =
            DependencyProperty.Register("ServiceProvider", typeof(IQRCodeLoginService), typeof(QRCodeLoginBehavior), new PropertyMetadata(null));

        /// <summary>
        /// 获取登录二维码
        /// </summary>
        public event Func<Task<QRCodeLoginResultModel>>? GetLoginQRCode;

        /// <summary>
        /// 查询二维码登录状态
        /// </summary>
        public event Func<Task<QRCodeLoginStatusModel>>? QueryQRCodeLoginStatus;

        /// <summary>
        /// 二维码授权登录成功
        /// </summary>
        public event Action<QRCodeLoginResultModel>? QRCodeAuthLoginSuccess;

        /// <summary>
        /// 取消二维码登录
        /// </summary>
        public event Action<QRCodeLoginResultModel>? CancelQRCodeLogin;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.ServiceProvider = this;
            this.AssociatedObject.GetLoginQRCode += AssociatedObject_GetLoginQRCode;
            this.AssociatedObject.QueryQRCodeLoginStatus += AssociatedObject_QueryQRCodeLoginStatus;
            this.AssociatedObject.QRCodeAuthLoginSuccess += AssociatedObject_QRCodeAuthLoginSuccess;
            this.AssociatedObject.CancelQRCodeLogin += AssociatedObject_CancelQRCodeLogin;
        }

        private void AssociatedObject_CancelQRCodeLogin(QRCodeLoginResultModel obj)
        {
            if (this.CancelQRCodeLogin == null)
            {
                throw new ArgumentNullException(nameof(CancelQRCodeLogin));
            }
            this.CancelQRCodeLogin.Invoke(obj);
        }

        private void AssociatedObject_QRCodeAuthLoginSuccess(QRCodeLoginResultModel obj)
        {
            if (this.QRCodeAuthLoginSuccess == null)
            {
                throw new ArgumentNullException(nameof(QRCodeAuthLoginSuccess));
            }
            this.QRCodeAuthLoginSuccess.Invoke(obj);
        }

        private async Task<QRCodeLoginStatusModel> AssociatedObject_QueryQRCodeLoginStatus()
        {
            if (this.QueryQRCodeLoginStatus == null)
            {
                throw new ArgumentNullException(nameof(QueryQRCodeLoginStatus));
            }

            return await this.QueryQRCodeLoginStatus.Invoke();
        }

        private Task<QRCodeLoginResultModel> AssociatedObject_GetLoginQRCode()
        {
            if (this.GetLoginQRCode == null)
            {
                throw new ArgumentNullException(nameof(GetLoginQRCode));
            }

            return this.GetLoginQRCode.Invoke();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.GetLoginQRCode -= GetLoginQRCode;
            this.AssociatedObject.QueryQRCodeLoginStatus -= QueryQRCodeLoginStatus;
            this.AssociatedObject.QRCodeAuthLoginSuccess -= QRCodeAuthLoginSuccess;
            this.AssociatedObject.CancelQRCodeLogin -= CancelQRCodeLogin;
        }
    }
}
