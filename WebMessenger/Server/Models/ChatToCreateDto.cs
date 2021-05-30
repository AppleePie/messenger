using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class ChatToCreateDto
    {
        [Required]
        public Guid InitiatorId { get; set; }
        [Required]
        public string InterlocutorName { get; set; }
    }
}