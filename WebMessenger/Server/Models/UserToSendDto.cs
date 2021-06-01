using System;
using System.Collections.Generic;

namespace Server.Models
{
    public class UserToSendDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public List<ChatForUser> Chats { get; set; }
    }
}