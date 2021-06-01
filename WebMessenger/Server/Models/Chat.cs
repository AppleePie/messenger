using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Server.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public List<ChatToMessage> ChatToMessages { get; set; } = new();
        public List<UserToChat> UserToChats { get; set; } = new();
        
        public string RelationsWithMessages { get; set; } = "";

        [NotMapped] public const string Delimiter = ";";

        [NotMapped]
        public IEnumerable<Guid> MyMessageIds => RelationsWithMessages.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries)
            .Select(Guid.Parse);
    }
}