using RS.Widgets.Models;

namespace RS.WPFClient.Client.IServices
{
    /// <summary>
    /// 此处只是一个示例 说明在应对第三方不支持MVVM事件绑定
    /// 还要在ViewModel里进行事件回调注册一个Demo说明
    /// </summary>
    public interface IQRCodeLoginService
    {
        /// <summary>
        /// 获取登录二维码
        /// </summary>
        event Func<Task<QRCodeLoginResultModel>>? GetLoginQRCode;

        /// <summary>
        /// 查询二维码登录状态
        /// </summary>
        event Func<Task<QRCodeLoginStatusModel>>? QueryQRCodeLoginStatus;

        /// <summary>
        /// 二维码授权登录成功
        /// </summary>
        event Action<QRCodeLoginResultModel>? QRCodeAuthLoginSuccess;

        /// <summary>
        /// 取消二维码登录
        /// </summary>
        event Action<QRCodeLoginResultModel>? CancelQRCodeLogin;
    }
}
