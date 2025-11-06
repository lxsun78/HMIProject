using Microsoft.Extensions.DependencyInjection;
using RS.Commons;
using RS.Commons.Attributs;
using RS.Commons.Extensions;
using RS.WPFClient.Client.IServices;
using RS.Models;
using RS.Server.WebAPI;
using RS.Widgets.Enums;
using RS.Widgets.Models;
using System.ComponentModel;

namespace RS.WPFClient.Client.ViewModels
{
    [ServiceInjectConfig(ServiceLifetime.Transient)]
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            this.SetPasswordLoginView();
        }

        #region 密码登录

        private void SetPasswordLoginView()
        {
            this.PasswordLoginViewModel = App.ServiceProvider.GetRequiredService<PasswordLoginViewModel>();
            if (this.PasswordLoginViewModel == null)
            {
                return;
            }
            this.PasswordLoginViewModel.OnForgetPassword+= PasswordLoginViewModel_OnForgetPassword;
            this.PasswordLoginViewModel.OnRegister += PasswordLoginViewModel_OnRegister;
            this.PasswordLoginViewModel.OnQRLogin += PasswordLoginViewModel_OnQRLogin;
            this.SetLoginContent(this.PasswordLoginViewModel);
        }

        private void PasswordLoginViewModel_OnQRLogin()
        {
            this.RemovePasswordLoginView();
            this.SetQRLoginView();
        }

        private void RemovePasswordLoginView()
        {
            if (this.PasswordLoginViewModel == null)
            {
                return;
            }
            this.PasswordLoginViewModel.OnForgetPassword -= PasswordLoginViewModel_OnForgetPassword;
            this.PasswordLoginViewModel.OnRegister -= PasswordLoginViewModel_OnRegister;
            this.PasswordLoginViewModel = null;
            this.SetLoginContent(this.PasswordLoginViewModel);
        }

        private void PasswordLoginViewModel_OnForgetPassword()
        {
            SetSecurityView();
        }

        private void PasswordLoginViewModel_OnRegister()
        {
            this.SetRegisterView();
        }

        #endregion

        #region 二维码登录
        private void SetQRLoginView()
        {
            this.QRLoginViewModel = App.ServiceProvider.GetRequiredService<QRLoginViewModel>();
            if (this.QRLoginViewModel == null)
            {
                return;
            }
            this.QRLoginViewModel.OnPasswordLogin += QRLoginViewModel_OnPasswordLogin;
            this.SetLoginContent(this.QRLoginViewModel);
        }

        private void RemoveQRLoginView()
        {
            if (this.QRLoginViewModel == null)
            {
                return;
            }
            this.QRLoginViewModel.OnPasswordLogin -= QRLoginViewModel_OnPasswordLogin;
            this.QRLoginViewModel = null;
            this.SetLoginContent(this.QRLoginViewModel);
        }

        private void QRLoginViewModel_OnPasswordLogin()
        {
            this.RemoveQRLoginView();
            this.SetPasswordLoginView();
        }
        #endregion


        #region 忘记密码

        private void SetSecurityView()
        {
            //这里每次都需要重新获取服务
            this.SecurityViewModel = App.ServiceProvider?.GetService<SecurityViewModel>();
            if (this.SecurityViewModel == null)
            {
                return;
            }
            this.SecurityViewModel.OnReturn += SecurityViewModel_OnReturn;
            this.SetLoginContent(this.SecurityViewModel);
        }

        private void RemoveSecurityView()
        {
            if (this.SecurityViewModel == null)
            {
                return;
            }
            this.SecurityViewModel.OnReturn -= SecurityViewModel_OnReturn;
            this.SecurityViewModel = null;
            this.SetLoginContent(this.SecurityViewModel);
        }


        /// <summary>
        /// 更改密码返回按钮点击事件
        /// </summary>
        private void SecurityViewModel_OnReturn()
        {
            this.RemoveSecurityView();
            this.SetPasswordLoginView();
        }

        #endregion


        #region 注册

        private void SetRegisterView()
        {
            this.RegisterViewModel = App.ServiceProvider.GetRequiredService<RegisterViewModel>();
            if (this.RegisterViewModel == null)
            {
                return;
            }

            this.RegisterViewModel.OnReturn += RegisterViewModel_OnReturn;
            this.SetLoginContent(this.RegisterViewModel);
        }

        private void RemoveRegisterView()
        {
            if (this.RegisterViewModel == null)
            {
                return;
            }
            this.RegisterViewModel.OnReturn -= RegisterViewModel_OnReturn;
            this.RegisterViewModel = null;
            this.LoginContent = null;
            this.SetLoginContent(this.RegisterViewModel);
        }

        private void RegisterViewModel_OnReturn()
        {
            this.RemoveRegisterView();
            this.SetPasswordLoginView();
        }
        #endregion


        private void SetLoginContent(INotifyPropertyChanged? notifyPropertyChanged)
        {
            this.LoginContent = notifyPropertyChanged;
        }


        private PasswordLoginViewModel? passwordLoginViewModel;
        /// <summary>
        /// 密码登录
        /// </summary>
        public PasswordLoginViewModel? PasswordLoginViewModel
        {
            get
            {
                return passwordLoginViewModel;
            }
            set
            {
                SetProperty(ref passwordLoginViewModel, value);
            }
        }

        private QRLoginViewModel? qrLoginViewModel;
        /// <summary>
        /// 二维码登录
        /// </summary>
        public QRLoginViewModel? QRLoginViewModel
        {
            get
            {
                return qrLoginViewModel;
            }
            set
            {
                SetProperty(ref qrLoginViewModel, value);
            }
        }


        private RegisterViewModel? registerViewModel;
        /// <summary>
        /// 注册
        /// </summary>
        public RegisterViewModel? RegisterViewModel
        {
            get
            {
                return registerViewModel;
            }
            set
            {
                SetProperty(ref registerViewModel, value);
            }
        }


        private SecurityViewModel? securityViewModel;
        /// <summary>
        /// 忘记密码
        /// </summary>
        public SecurityViewModel? SecurityViewModel
        {
            get
            {
                return securityViewModel;
            }
            set
            {
                SetProperty(ref securityViewModel, value);
            }
        }


        private INotifyPropertyChanged loginContent;
        /// <summary>
        /// 视图内容
        /// </summary>
        public INotifyPropertyChanged LoginContent
        {
            get { return loginContent; }
            set
            {
                SetProperty(ref loginContent, value);
            }
        }

    }
}
