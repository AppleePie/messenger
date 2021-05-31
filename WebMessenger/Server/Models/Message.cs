using System;

namespace Server.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        
        public UserToMessage UserToMessage { get; set; }
        public ChatToMessage ChatToMessage { get; set; }
    }
}