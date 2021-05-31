using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub, IChatClient> chatHub;

        public ChatController(IHubContext<ChatHub, IChatClient> chatHub)
        {
            this.chatHub = chatHub;
        }

        [HttpPost("messages")]
        public async Task Post(ChatMessage message)
        {
            await chatHub.Clients.Group(message.Initiator.ToString()).ReceiveMessage(message);
            await chatHub.Clients.Group(message.Interlocutor.ToString()).ReceiveMessage(message);
        }
    }
}