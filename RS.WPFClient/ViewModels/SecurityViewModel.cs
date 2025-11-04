using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RS.Commons.Attributs;
using RS.Commons.Extensions;
using RS.WPFClient.Client.Enums;
using RS.Models;
using RS.Server.WebAPI;
using RS.Widgets.Controls;
using RS.Widgets.Enums;
using RS.Widgets.Models;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace RS.WPFClient.Client.ViewModels
{
    [ServiceInjectConfig(ServiceLifetime.Transient)]
    public class SecurityViewModel : ViewModelBase
    {
        #region 自定义事件
        public event Action OnReturnExcute;
        #endregion

        public ICommand ReturnCommand { get; }
        public ICommand ReturnLoginCommand { get; }
        public ICommand SendEmailPasswordResetCommand { get; }

        public SecurityViewModel()
        {
            this.ReturnCommand = new RelayCommand(ReturnExcute);
            this.ReturnLoginCommand = new RelayCommand(ReturnLoginExcute);
            this.SendEmailPasswordResetCommand = new RelayCommand(SendEmailPasswordResetExcute);
        }

        private async void SendEmailPasswordResetExcute()
        {
            // 验证用户名和输入密码是否符合要求
            var validResult = this.ValidObject();
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
                var emailModel = new EmailModel();
                emailModel.Email = this.Email;
                //获取邮箱验证码结果
                var passwordResetEmailSendResult = await HMIWebAPI.Security.PasswordResetEmailSend.AESHttpPostAsync<EmailModel>(emailModel, nameof(HMIWebAPI));
                if (!passwordResetEmailSendResult.IsSuccess)
                {
                    return passwordResetEmailSendResult;
                }
                return passwordResetEmailSendResult;
            }, loadingConfig);

            //如果失败
            if (!operateResult.IsSuccess)
            {
                this.ParentWin.ShowInfoAsync(operateResult.Message, InfoType.Warning);
                return;
            }

            this.TaskStatus = SecurityTaskStatus.EmailSendSuccess;
        }

        private void ReturnLoginExcute()
        {
            this.OnReturnExcute?.Invoke();
        }

        private void ReturnExcute()
        {
            this.OnReturnExcute?.Invoke();
        }

        private SecurityTaskStatus taskStatus;
        /// <summary>
        /// 任务状态
        /// </summary>
        public SecurityTaskStatus TaskStatus
        {
            get { return taskStatus; }
            set
            {
                this.SetProperty(ref taskStatus, value);
            }
        }



        private string? email;
        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(50, ErrorMessage = "邮箱长度不能超过50")]
        [Required(ErrorMessage = "邮箱不能为空")]
        [RegularExpression("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])", ErrorMessage = "用户名格式不正确")]
        public string? Email
        {
            get { return email; }
            set
            {
                this.SetProperty(ref email, value);
                this.ValidProperty(value);
            }
        }
    }
}
