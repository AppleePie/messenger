using System;

namespace Server.Models
{
    public class ChatMessage
    {
        public Guid Initiator { get; set; }
        public Guid Interlocutor { get; set; }

        public string Message { get; set; }
    }
}