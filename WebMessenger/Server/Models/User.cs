using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Server.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<UserToChat> UserToChats { get; set; } = new();

        public string RelationsWithChats { get; set; } = "";

        [NotMapped] public const string Delimiter = ";";

        [NotMapped]
        public IEnumerable<Guid> MyChatIds => RelationsWithChats.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries)
            .Select(Guid.Parse);
    }
}