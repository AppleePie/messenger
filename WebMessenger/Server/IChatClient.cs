using System.Threading.Tasks;
using Server.Models;

namespace Server
{
    public interface IChatClient
    {
        Task ReceiveMessage(ChatMessage message);
    }
}