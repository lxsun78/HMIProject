using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xaml.Behaviors;
using RS.Commons;
using RS.WPFClient.Client.Controls;
using RS.WPFClient.Client.IServices;
using RS.Models;
using RS.Widgets.Behaviors;
using RS.Widgets.Controls;
using System.Threading.Tasks;
using System.Windows;

namespace RS.WPFClient.Client.Behaviors
{
    public class ImgVerifyBehavior : Behavior<RSImgVerify>, IImgVerifyService
    {
        public IImgVerifyService ServiceProvider
        {
            get { return (IImgVerifyService)GetValue(ServiceProvidereProperty); }
            set { SetValue(ServiceProvidereProperty, value); }
        }

        public static readonly DependencyProperty ServiceProvidereProperty =
            DependencyProperty.Register("ServiceProvider", typeof(IImgVerifyService), typeof(ImgVerifyBehavior), new PropertyMetadata(null));


        /// <summary>
        /// 初始化验证码事件
        /// </summary>
        public event Func<Task<OperateResult<ImgVerifyModel>>> InitVerifyControlAsyncEvent;

        /// <summary>
        /// 验证码拖拽开始事件
        /// </summary>
        public event Func<OperateResult> BtnSliderDragStartedEvent;


        protected override void OnAttached()
        {
            base.OnAttached();
            this.ServiceProvider = this;

            this.AssociatedObject.BtnSliderDragStartedEvent += AssociatedObject_BtnSliderDragStartedEvent;
            this.AssociatedObject.InitVerifyControlAsyncEvent += AssociatedObject_InitVerifyControlAsyncEvent;
        }

        private async Task<OperateResult<ImgVerifyModel>> AssociatedObject_InitVerifyControlAsyncEvent()
        {
            if (this.InitVerifyControlAsyncEvent == null)
            {
                throw new ArgumentNullException(nameof(InitVerifyControlAsyncEvent));
            }
            return await this.InitVerifyControlAsyncEvent.Invoke();
        }

        private OperateResult AssociatedObject_BtnSliderDragStartedEvent()
        {
            if (BtnSliderDragStartedEvent == null)
            {
                throw new ArgumentNullException(nameof(BtnSliderDragStartedEvent));
            }
            return this.BtnSliderDragStartedEvent.Invoke(); ;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        public async Task<OperateResult<ImgVerifyResultModel>> GetImgVerifyResultAsync()
        {
            return await this.AssociatedObject.GetImgVerifyResultAsync();
        }

        public async Task<OperateResult> ResetImgVerifyAsync()
        {
            return await this.AssociatedObject.ResetImgVerifyAsync();
        }

    }
}
