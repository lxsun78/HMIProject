using RS.Commons;
using RS.WPFClient.Client.Controls;
using RS.WPFClient.Client.Views;
using RS.Models;
using RS.Widgets.Controls;
using RS.Widgets.Interfaces;
using RS.Widgets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.WPFClient.Client.IServices
{
    public interface IImgVerifyService
    {
        Task<OperateResult<ImgVerifyResultModel>> GetImgVerifyResultAsync();
        Task<OperateResult> ResetImgVerifyAsync();

        /// <summary>
        /// 初始化验证码事件
        /// </summary>
        event Func<Task<OperateResult<ImgVerifyModel>>> InitVerifyControlAsyncEvent;

        /// <summary>
        /// 验证码拖拽开始事件
        /// </summary>
        event Func<OperateResult> BtnSliderDragStartedEvent;
    }
}
