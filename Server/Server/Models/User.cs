using System;

namespace Server.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string AvatarFilePath { get; set; }
    }
}