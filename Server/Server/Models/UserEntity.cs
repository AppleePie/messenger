using System;

namespace Server.Models
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string AvatarFilePath { get; set; }
    }
}