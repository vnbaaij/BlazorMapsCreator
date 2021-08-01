using System;

using BlazorFluentUI;

namespace BlazorMapsCreator.Models
{
    public class MessageItem
    {
        public string Id { get; } = Guid.NewGuid().ToString().Replace("-", "");
        public string Message { get; set; }
        public MessageBarType MessageType { get; set; }

        public MessageItem(string message, MessageBarType type = MessageBarType.Info)
        {
            Message = message;
            MessageType = type;
        }
    }
}
