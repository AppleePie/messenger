using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        [Required]
        [RegularExpression("^[0-9\\p{L}]*$", ErrorMessage = "Login should contain only letters or digits")]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        public string AvatarFilePath { get; set; }
    }
}