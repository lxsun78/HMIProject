using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RS.Commons;
using RS.Commons.Attributs;
using RS.WPFClient.Client.IServices;
using RS.WPFClient.Client.Models;
using RS.WPFClient.IBLL;
using RS.Models;
using RS.Server.WebAPI;
using RS.Widgets.Enums;
using RS.Widgets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RS.WPFClient.Client.ViewModels
{
    [ServiceInjectConfig(ServiceLifetime.Transient)]
    public class PasswordLoginViewModel : ViewModelBase
    {
        private readonly IGeneralBLL GeneralBLL;
        private readonly ICryptographyBLL CryptographyBLL;

        #region 自定义事件
        public event Action OnRegisterExcute;
        public event Action OnForgetPasswordExcute;
        public event Action OnQRLoginExcute;
        #endregion

        /// <summary>
        /// 登录
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// 注册
        /// </summary>
        public ICommand RegisterCommand { get; }

        /// <summary>
        /// 忘记密码
        /// </summary>
        public ICommand ForgetPasswordCommand { get; }

        public ICommand QRLoginCommand { get; }
        public PasswordLoginViewModel(IGeneralBLL generalBLL,
            ICryptographyBLL cryptographyBLL)
        {
            LoginCommand = new RelayCommand(LoginExcute);
            ForgetPasswordCommand = new RelayCommand(ForgetPasswordExcute);
            RegisterCommand = new RelayCommand(RegisterExcute);
            QRLoginCommand = new RelayCommand(QRLoginExcute);
        }

        private void QRLoginExcute()
        {
            OnQRLoginExcute?.Invoke();
        }

        /// <summary>
        /// 注册按钮点击事件
        /// </summary>
        private void RegisterExcute()
        {
            OnRegisterExcute?.Invoke();
        }

        /// <summary>
        /// 忘记密码点击事件
        /// </summary>
        private void ForgetPasswordExcute()
        {
            OnForgetPasswordExcute?.Invoke();
        }


        /// <summary>
        /// 登录点击事件
        /// </summary>
        private async void LoginExcute()
        {
            //var loadingConfig = new LoadingConfig()
            //{
            //    LoadingType = LoadingType.ProgressBar,
            //    //Minimum = 0,
            //    Maximum = 100,
            //    Value = 0,
            //    IsIndeterminate = true,
            //    //IconWidth = 25,
            //    //IconHeight = 32,
            //    //LoadingBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88000000")),
            //    //LoadingColor = new SolidColorBrush(Colors.Red),
            //    //IconData = Geometry.Parse("M512 973.653333a34.133333 34.133333 0 0 1-24.132267-58.2656l27.050667-27.067733h-3.771733c-102.8096 0-191.8976-36.949333-264.772267-109.841067S136.533333 616.209067 136.533333 512.853333c0-69.6832 17.134933-133.597867 50.944-189.934933a34.133333 34.133333 0 0 1 58.538667 35.1232C218.658133 403.626667 204.8 455.714133 204.8 512.853333c0 85.794133 29.3888 156.893867 89.838933 217.361067 60.450133 60.450133 131.2768 89.838933 216.507734 89.838933h3.771733l-27.050667-27.067733a34.133333 34.133333 0 1 1 48.264534-48.264533l85.282133 85.282133 0.529067 0.529067c3.0208 3.140267 5.307733 6.7072 6.877866 10.496a34.030933 34.030933 0 0 1-7.355733 37.290666l-85.333333 85.333334c-6.673067 6.656-15.394133 10.001067-24.132267 10.001066z m297.7792-257.706666a34.1504 34.1504 0 0 1-29.508267-51.2512C806.0928 620.1344 819.2 569.0368 819.2 512.853333c0-85.794133-29.3888-156.893867-89.838933-217.361066-60.4672-60.450133-131.293867-89.838933-216.541867-89.838934h-3.191467l27.221334 26.948267a34.133333 34.133333 0 0 1-48.0256 48.520533l-86.186667-85.333333-0.8704-0.887467-0.017067-0.034133-0.034133-0.034133a34.133333 34.133333 0 0 1 0.8704-47.496534l86.135467-86.1184a34.133333 34.133333 0 1 1 48.264533 48.264534L509.0816 137.386667h3.754667c102.8096 0 191.914667 36.9664 264.8064 109.841066S887.466667 409.480533 887.466667 512.853333c0 68.369067-16.1792 130.9696-48.110934 186.077867a34.116267 34.116267 0 0 1-29.576533 17.015467z")
            //};

            //var operateResult = await Loading.InvokeAsync(async (cancellationToken) =>
            //{
            //    // 验证用户名和输入密码是否符合要求
            //    var validResult = LoginModel.ValidObject();
            //    if (!validResult)
            //    {
            //        return OperateResult.CreateFailResult("输入数据验证失败！");
            //    }

            //    OperateResult<ImgVerifyResultModel> getImgVerifyResult = null;

            //    if (ImgVerifyService != null)
            //    {
            //        //获取验证码
            //        getImgVerifyResult = await ImgVerifyService.GetImgVerifyResultAsync();
            //    }

            //    if (getImgVerifyResult == null)
            //    {
            //        return OperateResult.CreateFailResult("获取验证码失败！");
            //    }

            //    if (!getImgVerifyResult.IsSuccess)
            //    {
            //        return getImgVerifyResult;
            //    }

            //    var imgVerifyResultModel = getImgVerifyResult.Data;

            //    //验证用户登录
            //    var validLoginResult = await HMIWebAPI.Security.ValidLogin.AESHttpPostAsync(new LoginValidModel()
            //    {
            //        Email = LoginModel.Email,
            //        Password = CryptographyBLL.GetSHA256HashCode(LoginModel.Password),
            //        Verify = imgVerifyResultModel.Verify,
            //        VerifySessionId = imgVerifyResultModel.VerifySessionId,
            //    }, nameof(HMIWebAPI));

            //    return validLoginResult;
            //}, loadingConfig);


            ////如果验证失败
            //if (!operateResult.IsSuccess)
            //{
            //    await ShowMessageAsync(operateResult.Message);

            //    operateResult = await ImgVerifyService.ResetImgVerifyAsync();
            //    if (!operateResult.IsSuccess)
            //    {
            //        await ShowMessageAsync(operateResult.Message);
            //    }
            //    return;
            //}

            //operateResult = await LoginViewService.CloseAsync();
            //if (!operateResult.IsSuccess)
            //{
            //    await ShowMessageAsync(operateResult.Message);
            //    return;
            //}

            //var homeView = App.ServiceProvider?.GetService<HomeView>();
            //homeView?.Show();
        }

        private LoginModel? loginModel;
        /// <summary>
        /// 登录实体
        /// </summary>
        public LoginModel? LoginModel
        {
            get
            {
                if (loginModel == null)
                {
                    loginModel = new LoginModel();
                }
                return loginModel;
            }
            set
            {
                SetProperty(ref loginModel, value);
            }
        }
    }
}
