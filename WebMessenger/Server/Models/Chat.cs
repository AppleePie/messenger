using System;
using System.Collections.Generic;

namespace Server.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public List<ChatToMessage> ChatToMessages { get; set; } = new();
        public List<UserToChat> UserToChats { get; set; } = new();
    }
}