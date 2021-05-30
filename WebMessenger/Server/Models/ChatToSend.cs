using System;
using System.Collections.Generic;

namespace Server.Models
{
    public class ChatToSend
    {
        public Guid Id { get; set; }
        public List<Guid> Participants { get; set; }
    }
}