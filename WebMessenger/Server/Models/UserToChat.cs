using System;

namespace Server.Models
{
    public class UserToChat
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}