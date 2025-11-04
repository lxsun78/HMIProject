using RS.Commons;

namespace RS.WPFClient.Client.IServices
{
    public interface ILoginViewService
    {
        Task<OperateResult> CloseAsync();
    }
}
