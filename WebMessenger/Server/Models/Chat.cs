using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Server.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public List<UserToChat> UserToChats { get; set; } = new();
    }
}