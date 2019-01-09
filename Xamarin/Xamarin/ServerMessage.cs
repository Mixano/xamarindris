using System;
using System.Runtime.Serialization;

namespace Xamarin
{
    [DataContract]
    class ServerMessage
    {
        [DataMember]
        public string Message { get; private set; }

        [DataMember]
        public ServerMessageType MessageType { get; private set; }

        [DataMember]
        public DateTime Time { get; private set; }

        public ServerMessage(string message, ServerMessageType messageType, DateTime time)
        {
            Message = message;
            MessageType = messageType;
            Time = time;
        }
    }
}
