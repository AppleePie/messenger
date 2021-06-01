using System;

namespace Server.Models
{
    public class ChatToMessage
    {
        public Guid Id { get; set; }

        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
        
        public Guid MessageId { get; set; }
        public Message Message { get; set; }
    }
}