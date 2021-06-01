using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class UserToUpdateDto
    {
        [RegularExpression("^[0-9\\p{L}]*$", ErrorMessage = "Login should contain only letters or digits")]
        public string Login { get; set; }
        public string Password { get; set; }
    }
}