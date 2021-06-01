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
        }
    }
}