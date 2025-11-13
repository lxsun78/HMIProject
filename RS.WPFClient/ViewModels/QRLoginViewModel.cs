using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RS.Commons;
using RS.Commons.Attributs;
using RS.Models;
using RS.Server.WebAPI;
using RS.Widgets.Enums;
using RS.Widgets.Interfaces;
using RS.Widgets.Models;
using RS.WPFClient.IServices;
using RS.WPFClient.Models;
using RS.WPFClient.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RS.WPFClient.ViewModels
{
    [ServiceInjectConfig(ServiceLifetime.Transient)]
    public class QRLoginViewModel : ViewModelBase
    {
        #region 依赖注入服务
        private readonly IGeneralBLL GeneralBLL;
        private readonly ICryptographyBLL CryptographyBLL;
        private readonly IWindowService WindowService;
        #endregion


        #region 事件
        public event Action OnPasswordLogin;
        #endregion

        #region 命令
        public ICommand PasswordLoginCommand { get; set; }
        #endregion




        public QRLoginViewModel(IGeneralBLL generalBLL,
            ICryptographyBLL cryptographyBLL)
        {
            
            this.PasswordLoginCommand = new RelayCommand(PasswordLogin);
            this.CancelQRCodeLoginAction = CancelQRCodeLogin;
            this.QRCodeAuthLoginSuccessAction = QRCodeAuthLoginSuccess;
            this.GetLoginQRCodeAsyncFunc = GetLoginQRCodeAsync;
            this.QueryQRCodeLoginStatusAsyncFunc = QueryQRCodeLoginStatusAsync;

            this.TestAction = Test;
            this.IsQRCodeLogin = true;
        }

        private void Test(bool obj)
        {
            throw new NotImplementedException();
        }

        private Action<QRCodeLoginResultModel>? cancelQRCodeLoginAction;

        public Action<QRCodeLoginResultModel>? CancelQRCodeLoginAction
        {
            get { return cancelQRCodeLoginAction; }
            set
            {
                this.SetProperty(ref cancelQRCodeLoginAction, value);
            }
        }


        private Func<Task<QRCodeLoginResultModel>>? getLoginQRCodeAsyncFunc;

        public Func<Task<QRCodeLoginResultModel>>? GetLoginQRCodeAsyncFunc
        {
            get { return getLoginQRCodeAsyncFunc; }
            set
            {
                this.SetProperty(ref getLoginQRCodeAsyncFunc, value);
            }
        }
      
        private Action<QRCodeLoginResultModel>? qrCodeAuthLoginSuccessAction;

        public Action<QRCodeLoginResultModel>? QRCodeAuthLoginSuccessAction
        {
            get { return qrCodeAuthLoginSuccessAction; }
            set
            {
                this.SetProperty(ref qrCodeAuthLoginSuccessAction, value);
            }
        }



        private Func<Task<QRCodeLoginStatusModel>>? queryQRCodeLoginStatusAsyncFunc;

         
        public Func<Task<QRCodeLoginStatusModel>>? QueryQRCodeLoginStatusAsyncFunc
        {
            get { return queryQRCodeLoginStatusAsyncFunc; }
            set
            {
                this.SetProperty(ref queryQRCodeLoginStatusAsyncFunc, value);
            }
        }


        private Action<bool> testAction;


        public Action<bool> TestAction
        {
            get { return testAction; }
            set
            {
                this.SetProperty(ref testAction, value);
            }
        }
        



        public async Task<QRCodeLoginStatusModel> QueryQRCodeLoginStatusAsync()
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



        public async Task<QRCodeLoginResultModel> GetLoginQRCodeAsync()
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


        public void QRCodeAuthLoginSuccess(QRCodeLoginResultModel obj)
        {
            Loading.InvokeAsync(async (cancellationToken) =>
            {
                //待实现
                return OperateResult.CreateSuccessResult();
            });
        }

        public void CancelQRCodeLogin(QRCodeLoginResultModel model)
        {

        }

        private void PasswordLogin()
        {
            OnPasswordLogin?.Invoke();
        }

        private bool isQRCodeLogin;

        public bool IsQRCodeLogin
        {
            get { return isQRCodeLogin; }
            set
            {
                this.SetProperty(ref isQRCodeLogin, value);
            }
        }

    }
}
