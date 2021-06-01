using System;

namespace Server.Models
{
    public class ChatForUser
    {
        public Guid Id { get; set; }

        public Guid Interlocutor { get; set; } = new();
    }
}