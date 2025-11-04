using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RS.Commons;
using RS.Commons.Attributs;
using RS.Commons.Extensions;
using RS.WPFClient.Client.Enums;
using RS.WPFClient.Client.Models;
using RS.Models;
using RS.Server.WebAPI;
using RS.Widgets.Controls;
using RS.Widgets.Enums;
using RS.Widgets.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace RS.WPFClient.Client.ViewModels
{
    [ServiceInjectConfig(ServiceLifetime.Transient)]
    public class RegisterViewModel : ViewModelBase
    {
        private readonly ICryptographyBLL CryptographyBLL;
        private RegisterVerifyModel RegisterVerifyModel;
        private DispatcherTimer DispatcherTimer;


        #region 自定义事件
        public event Action OnReturnExcute;
        #endregion


        public ICommand SignUpRequestCommand { get; }
        public ICommand VerifyConfirmCommand { get; }
        public ICommand ReturnCommand { get; }
        public ICommand ReturnLoginCommand { get; }


        public RegisterViewModel(ICryptographyBLL cryptographyBLL)
        {
            CryptographyBLL = cryptographyBLL;
            this.SignUpRequestCommand = new RelayCommand(SignUpRequestExcute);
            this.VerifyConfirmCommand = new RelayCommand(VerifyConfirmExcute);
            this.ReturnCommand = new RelayCommand(ReturnExcute);
            this.ReturnLoginCommand = new RelayCommand(ReturnLoginExcute);

            this.VerifyCodeModelList = new ObservableCollection<VerifyCodeModel>();

            for (int i = 0; i < 6; i++)
            {
                this.VerifyCodeModelList.Add(new VerifyCodeModel()
                {
                    VerifyCodeKeyDownCommand = new RelayCommand<VerifyCodeModel>(VerifyCodeKeyDown),
                    VerifyCodeTextChangedCommand = new RelayCommand<VerifyCodeModel>(VerifyCodeTextChanged)
                });
            }

            this.TaskStatus = RegisterTaskStatus.RegisterForm;
        }

        private void VerifyCodeTextChanged(VerifyCodeModel verifyCodeModel)
        {
            var index = this.VerifyCodeModelList.IndexOf(verifyCodeModel);
            if (!string.IsNullOrEmpty(verifyCodeModel.Text) && index < 5)
            {
                this.VerifyCodeModelList[index + 1].IsFocused = true;
            }
        }

        private void VerifyCodeKeyDown(VerifyCodeModel verifyCodeModel)
        {

            var index = this.VerifyCodeModelList.IndexOf(verifyCodeModel);
            if (Keyboard.IsKeyDown(Key.Back))
            {
                verifyCodeModel.Text = null;
                if (index > 0)
                {
                    this.VerifyCodeModelList[index - 1].IsFocused = true;
                }
            }
            else if (
                (Keyboard.IsKeyDown(Key.LeftCtrl)
                || Keyboard.IsKeyDown(Key.RightCtrl))
                && Keyboard.IsKeyDown(Key.V)
                )
            {
                IDataObject data = Clipboard.GetDataObject();
                if (data != null && data.GetDataPresent(DataFormats.Text))
                {
                    string clipboardText = data.GetData(DataFormats.Text)?.ToString();
                    string replacedText = clipboardText.Replace(Environment.NewLine, "").Replace(" ", "");
                    var verifyCodeList = replacedText.Take(6).ToList();
                    for (int i = 0; i < verifyCodeList.Count; i++)
                    {
                        var code = verifyCodeList[i].ToString();
                        var item = this.VerifyCodeModelList[i];
                        item.Text = code;
                        item.IsFocused = true;
                        item.CaretIndex = 1;
                    }
                }
            }
        }

        private void ReturnLoginExcute()
        {
            this.OnReturnExcute?.Invoke();
        }

        private void ReturnExcute()
        {
            switch (this.TaskStatus)
            {
                case RegisterTaskStatus.RegisterForm:
                    this.OnReturnExcute?.Invoke();
                    break;
                case RegisterTaskStatus.EmailVerify:
                    //停止定时器
                    if (this.DispatcherTimer != null)
                    {
                        this.DispatcherTimer.Stop();
                        this.DispatcherTimer = null;
                    }
                    //清除验证码信息
                    foreach (var item in this.VerifyCodeModelList)
                    {
                        item.Text = null;
                    }
                    this.TaskStatus = RegisterTaskStatus.RegisterForm;
                    break;
                case RegisterTaskStatus.RegisterSuccess:
                    break;
            }
        }

        private async void VerifyConfirmExcute()
        {
            var textList = VerifyCodeModelList.Select(t => t.Text).ToList();
            var verify = string.Join("", textList);

            if (verify.Length != 6)
            {
                this.ParentWin?.ShowInfoAsync("验证码验证失败！", InfoType.Warning);
                return;
            }

            var loadingConfig = new LoadingConfig()
            {
                LoadingType = LoadingType.ProgressBar,
                Maximum = 100,
                Value = 0,
                IsIndeterminate = true,
            };

            var operateResult = await this.Loading.InvokeAsync(async (cancellationToken) =>
            {

                var registerVerifyValidModel = new RegisterVerifyValidModel();
                registerVerifyValidModel.RegisterSessionId = this.RegisterVerifyModel.RegisterSessionId;
                registerVerifyValidModel.Verify = verify;


                //获取邮箱验证码结果
                var emailVerifyValidResult = await HMIWebAPI.Register.EmailVerifyValid.AESHttpPostAsync<RegisterVerifyValidModel>(registerVerifyValidModel, nameof(HMIWebAPI), RegisterVerifyModel.Token);

                if (!emailVerifyValidResult.IsSuccess)
                {
                    return emailVerifyValidResult;
                }

                return emailVerifyValidResult;
            }, loadingConfig);

            //如果失败
            if (!operateResult.IsSuccess)
            {
                this.ParentWin?.ShowInfoAsync(operateResult.Message, InfoType.Warning);
                return;
            }

            this.TaskStatus = RegisterTaskStatus.RegisterSuccess;
        }

        private async void SignUpRequestExcute()
        {
            //注册信息验证
            var validResult = this.SignUpModel.ValidObject();
            if (!validResult)
            {
                return;
            }

            var loadingConfig = new LoadingConfig()
            {
                LoadingType = LoadingType.ProgressBar,
                Maximum = 100,
                Value = 0,
                IsIndeterminate = true,
            };


            var operateResult = await this.Loading.InvokeAsync(async (cancellationToken) =>
            {
                var emailRegisterPostModel = new EmailRegisterPostModel();
                emailRegisterPostModel.Email = this.SignUpModel.Email;
                emailRegisterPostModel.Password = this.CryptographyBLL.GetSHA256HashCode(this.SignUpModel.PasswordConfirm);

                //获取邮箱验证码结果
                var getEmailVerifyResult = await HMIWebAPI.Register.GetEmailVerify.AESHttpPostAsync<EmailRegisterPostModel, RegisterVerifyModel>(emailRegisterPostModel, nameof(HMIWebAPI));

                if (!getEmailVerifyResult.IsSuccess)
                {
                    return getEmailVerifyResult;
                }

                return getEmailVerifyResult;
            }, loadingConfig);

            //如果失败
            if (!operateResult.IsSuccess)
            {
                this.ParentWin?.ShowInfoAsync(operateResult.Message, InfoType.Warning);
                return;
            }

            this.RegisterVerifyModel = operateResult.Data;

            var expireTime = this.RegisterVerifyModel.ExpireTime.FromUnixTimeStamp();

            TimeSpan remainingTime = expireTime - DateTime.Now;
            this.RemainingTime = remainingTime.TotalSeconds;
            this.DispatcherTimer = new DispatcherTimer();
            this.DispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            this.DispatcherTimer.Tick += (s, e) =>
            {
                remainingTime = expireTime - DateTime.Now;
                if (remainingTime.TotalSeconds > 0)
                {
                    this.RemainingTime = remainingTime.TotalSeconds;
                }
                else
                {
                    this.ReturnExcute();
                }
            };
            this.DispatcherTimer.Start();

            this.TaskStatus = RegisterTaskStatus.EmailVerify;
        }


        private RegisterTaskStatus taskStatus;
        /// <summary>
        /// 任务状态
        /// </summary>
        public RegisterTaskStatus TaskStatus
        {
            get
            {
                return taskStatus;
            }
            set
            {
                this.SetProperty(ref taskStatus, value);
            }
        }


        private double remainingTime;
        /// <summary>
        /// 剩余时间
        /// </summary>
        public double RemainingTime
        {
            get { return remainingTime; }
            set
            {
                this.SetProperty(ref remainingTime, value);
            }
        }


        private SignUpModel signUpModel;
        /// <summary>
        /// 注册
        /// </summary>
        public SignUpModel SignUpModel
        {
            get
            {
                if (signUpModel == null)
                {
                    signUpModel = new SignUpModel();
                }
                return signUpModel;
            }
            set
            {
                this.SetProperty(ref signUpModel, value);
            }
        }

        private ObservableCollection<VerifyCodeModel> verifyCodeModelList;

        public ObservableCollection<VerifyCodeModel> VerifyCodeModelList
        {
            get { return verifyCodeModelList; }
            set
            {
                this.SetProperty(ref verifyCodeModelList, value);
            }
        }
    }
}
