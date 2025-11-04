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

        private void LoginViewModel_PropertyChanging(object? sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case nameof(QRCodeLoginService):
            //        UnRegisterQRCodeLoginServiceCallbacks(QRCodeLoginService);
            //        break;
            //    case nameof(ImgVerifyService):
            //        UnRegisterImgVerifyServiceCallbacks(ImgVerifyService);
            //        break;
            //}
        }

        private void LoginViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case nameof(QRCodeLoginService):
            //        RegisterQRCodeLoginServiceCallbacks(QRCodeLoginService);
            //        break;
            //    case nameof(ImgVerifyService):
            //        RegisterImgVerifyServiceCallbacks(ImgVerifyService);
            //        break;
            //}
        }


        private void RegisterImgVerifyServiceCallbacks(IImgVerifyService? imgVerifyService)
        {
            if (imgVerifyService == null)
            {
                return;
            }

            imgVerifyService.BtnSliderDragStartedEvent += ImgVerifyService_BtnSliderDragStartedEvent;
            imgVerifyService.InitVerifyControlAsyncEvent += ImgVerifyService_InitVerifyControlAsyncEvent;
        }

        private void UnRegisterImgVerifyServiceCallbacks(IImgVerifyService? imgVerifyService)
        {
            if (imgVerifyService == null)
            {
                return;
            }
            imgVerifyService.BtnSliderDragStartedEvent -= ImgVerifyService_BtnSliderDragStartedEvent;
            imgVerifyService.InitVerifyControlAsyncEvent -= ImgVerifyService_InitVerifyControlAsyncEvent;
        }


        private void RegisterQRCodeLoginServiceCallbacks(IQRCodeLoginService? qrCodeLoginService)
        {
            if (qrCodeLoginService == null)
            {
                return;
            }
            qrCodeLoginService.QRCodeAuthLoginSuccess += QrCodeLoginService_QRCodeAuthLoginSuccess;
            qrCodeLoginService.CancelQRCodeLogin += QrCodeLoginService_CancelQRCodeLogin;
            qrCodeLoginService.GetLoginQRCode += QrCodeLoginService_GetLoginQRCode;
            qrCodeLoginService.QueryQRCodeLoginStatus += QrCodeLoginService_QueryQRCodeLoginStatus;
        }


        private void UnRegisterQRCodeLoginServiceCallbacks(IQRCodeLoginService? qrCodeLoginService)
        {
            if (qrCodeLoginService == null)
            {
                return;
            }
            qrCodeLoginService.QRCodeAuthLoginSuccess -= QrCodeLoginService_QRCodeAuthLoginSuccess;
            qrCodeLoginService.CancelQRCodeLogin -= QrCodeLoginService_CancelQRCodeLogin;
            qrCodeLoginService.GetLoginQRCode -= QrCodeLoginService_GetLoginQRCode;
            qrCodeLoginService.QueryQRCodeLoginStatus -= QrCodeLoginService_QueryQRCodeLoginStatus;

        }


        /// <summary>
        /// 请求获取滑动图像验证数据
        /// </summary>
        /// <returns></returns>
        private async Task<OperateResult<ImgVerifyModel>> ImgVerifyService_InitVerifyControlAsyncEvent()
        {
            var loadingConfig = new LoadingConfig()
            {
                LoadingType = LoadingType.ProgressBar,
                //Minimum = 0,
                Maximum = 100,
                Value = 0,
                IsIndeterminate = true,
            };

            var getImgVerifyModelResult = await Loading.InvokeAsync(async (cancellationToken) =>
            {
                return await HMIWebAPI.Security.GetImgVerifyModel.AESHttpGetAsync<ImgVerifyModel>(nameof(HMIWebAPI));
            }, loadingConfig);

            if (!getImgVerifyModelResult.IsSuccess)
            {
                ParentWin.ShowInfoAsync(getImgVerifyModelResult.Message, InfoType.Error);
            }

            return getImgVerifyModelResult;
        }

        private OperateResult ImgVerifyService_BtnSliderDragStartedEvent()
        {
            //// 验证用户名和输入密码是否符合要求
            //var validResult = LoginModel.ValidObject();
            //if (!validResult)
            //{
            //    return OperateResult.CreateFailResult();
            //}
            return OperateResult.CreateSuccessResult();
        }



        private async Task<QRCodeLoginStatusModel> QrCodeLoginService_QueryQRCodeLoginStatus()
        {
            var operateResult = await Loading.InvokeAsync(async (cancellationToken) =>
            {
                return OperateResult.CreateSuccessResult(new QRCodeLoginStatusModel());
            });

            if (!operateResult.IsSuccess)
            {
                ParentWin.ShowInfoAsync(operateResult.Message, InfoType.Error);
            }
            return operateResult.Data;
        }

        private async Task<QRCodeLoginResultModel> QrCodeLoginService_GetLoginQRCode()
        {
            var operateResult = await Loading.InvokeAsync(async (cancellationToken) =>
            {
                //用户就在这里去往服务端发起请求获取验证码
                var expireTime = new DateTimeOffset(DateTime.Now.AddSeconds(120)).ToUnixTimeMilliseconds();
                //https://passport.iqiyi.com/apis/qrcode/token_login.action?token=7a068e22fe923ea273bcf76242db4bfba
                string token = $"{Guid.NewGuid().ToString()}";
                QRCodeLoginResultModel loginQRCodeResultModel = new QRCodeLoginResultModel()
                {
                    IsSuccess = true,
                    Token = token,
                    QRCodeContent = $"https://passport.myweb.com/apis/qrcode/token_login?token={token}",
                    ExpireTime = expireTime
                };
                return OperateResult.CreateSuccessResult(loginQRCodeResultModel);
            });

            if (!operateResult.IsSuccess)
            {
                ParentWin.ShowInfoAsync(operateResult.Message, InfoType.Error);
            }
            return operateResult.Data;
        }

        private void QrCodeLoginService_CancelQRCodeLogin(QRCodeLoginResultModel obj)
        {

        }

        private void QrCodeLoginService_QRCodeAuthLoginSuccess(QRCodeLoginResultModel obj)
        {
            Loading.InvokeAsync(async (cancellationToken) =>
            {
                //待实现
                return OperateResult.CreateSuccessResult();
            });
        }


        #region 密码登录

        private void SetPasswordLoginView()
        {
            this.PasswordLoginViewModel = App.ServiceProvider.GetRequiredService<PasswordLoginViewModel>();
            if (this.PasswordLoginViewModel == null)
            {
                return;
            }
            this.PasswordLoginViewModel.OnForgetPasswordExcute += PasswordLoginViewModel_OnForgetPasswordExcute;
            this.PasswordLoginViewModel.OnRegisterExcute += PasswordLoginViewModel_OnRegisterExcute;
            this.PasswordLoginViewModel.OnQRLoginExcute += PasswordLoginViewModel_OnQRLoginExcute;
            this.SetLoginContent(this.PasswordLoginViewModel);
        }

        private void PasswordLoginViewModel_OnQRLoginExcute()
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
            this.PasswordLoginViewModel.OnForgetPasswordExcute -= PasswordLoginViewModel_OnForgetPasswordExcute;
            this.PasswordLoginViewModel.OnRegisterExcute -= PasswordLoginViewModel_OnRegisterExcute;
            this.PasswordLoginViewModel = null;
            this.SetLoginContent(this.PasswordLoginViewModel);
        }

        private void PasswordLoginViewModel_OnForgetPasswordExcute()
        {
            SetSecurityView();
        }

        private void PasswordLoginViewModel_OnRegisterExcute()
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
            this.QRLoginViewModel.OnPasswordLoginExcute += QRLoginViewModel_OnPasswordLoginExcute;
            this.SetLoginContent(this.QRLoginViewModel);
        }

        private void RemoveQRLoginView()
        {
            if (this.QRLoginViewModel == null)
            {
                return;
            }
            this.QRLoginViewModel.OnPasswordLoginExcute -= QRLoginViewModel_OnPasswordLoginExcute;
            this.QRLoginViewModel = null;
            this.SetLoginContent(this.QRLoginViewModel);
        }

        private void QRLoginViewModel_OnPasswordLoginExcute()
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
            this.SecurityViewModel.OnReturnExcute += SecurityViewModel_OnReturnExcute;
            this.SetLoginContent(this.SecurityViewModel);
        }

        private void RemoveSecurityView()
        {
            if (this.SecurityViewModel == null)
            {
                return;
            }
            this.SecurityViewModel.OnReturnExcute -= SecurityViewModel_OnReturnExcute;
            this.SecurityViewModel = null;
            this.SetLoginContent(this.SecurityViewModel);
        }


        /// <summary>
        /// 更改密码返回按钮点击事件
        /// </summary>
        private void SecurityViewModel_OnReturnExcute()
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

            this.RegisterViewModel.OnReturnExcute += RegisterViewModel_OnReturnExcute;
            this.SetLoginContent(this.RegisterViewModel);
        }

        private void RemoveRegisterView()
        {
            if (this.RegisterViewModel == null)
            {
                return;
            }
            this.RegisterViewModel.OnReturnExcute -= RegisterViewModel_OnReturnExcute;
            this.RegisterViewModel = null;
            this.LoginContent = null;
            this.SetLoginContent(this.RegisterViewModel);
        }

        private void RegisterViewModel_OnReturnExcute()
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
