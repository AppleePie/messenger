using System;

namespace Server.Models
{
    public class ChatToSendDto
    {
        public Guid Id { get; set; }

        public Guid Interlocutor { get; set; } = new();
    }
}