using System.Runtime.Serialization;

namespace Xamarin
{
    [DataContract]
    class ClientMessage
    {
        [DataMember]
        public string Message { get; private set; }

        [DataMember]
        public ClientMessageType MessageType { get; private set; }

        public ClientMessage(string message, ClientMessageType messageType)
        {
            Message = message;
            MessageType = messageType;
        }
    }
}
