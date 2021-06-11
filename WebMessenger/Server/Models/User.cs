using System;
using System.Collections.Generic;

namespace Server.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<UserToMessage> UserToMessages { get; set; }
        public List<UserToChat> UserToChats { get; set; } = new();
    }
}