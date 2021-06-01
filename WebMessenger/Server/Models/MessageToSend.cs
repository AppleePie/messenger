using System;

namespace Server.Models
{
    public class MessageToSend
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}