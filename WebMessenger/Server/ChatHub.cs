using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Models;

namespace Server
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task Send(string id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, id);
            await Clients.Group(id).ReceiveMessage(new ChatMessage
            {
                Initiator = Guid.Empty, 
                Interlocutor = Guid.Empty, 
                Message = $"Hello, {id}!"
            });
        }
    }
}