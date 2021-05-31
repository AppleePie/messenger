using System;

namespace Server.Models
{
    public class UserToMessage
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
        
        public Guid MessageId { get; set; }
        public Message Message { get; set; }
    }
}