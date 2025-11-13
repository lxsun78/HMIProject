using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RS.WPFClient.Messages
{
    
    public class DataRequestMessage<T,D> : AsyncRequestMessage<T>
    {
        public D Data { get; set; }
    }
}
