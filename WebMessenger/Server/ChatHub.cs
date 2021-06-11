using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Server
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task Send(string id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, id);
        }
    }
}