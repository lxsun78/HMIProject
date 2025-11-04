using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RS.WPFClient.Client.Messages
{
    
    public class DataRequestMessage<T,D> : AsyncRequestMessage<T>
    {
        public D Data { get; set; }
    }
}
