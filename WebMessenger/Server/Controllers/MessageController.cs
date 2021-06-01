using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.DatabaseWorkers;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/messages/{chatId:guid}")]
    public class MessageController : Controller
    {
        private readonly IHubContext<ChatHub, IChatClient> chatHub;
        private readonly Repository repository;

        public MessageController(IHubContext<ChatHub, IChatClient> chatHub, Repository repository)
        {
            this.chatHub = chatHub;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessages([FromRoute] Guid chatId)
        {
            var chat = await repository.FindByIdAsync<Chat>(chatId);
            if (chat is null)
                return NotFound();

            return Ok(chat);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] Guid chatId, [FromBody] ChatMessage message)
        {
            await chatHub.Clients.Group(message.Initiator.ToString()).ReceiveMessage(message);
            await chatHub.Clients.Group(message.Interlocutor.ToString()).ReceiveMessage(message);

            var owner = await repository.FindByIdAsync<User>(message.Initiator);
            var chat = await repository.FindByIdAsync<Chat>(chatId);

            var newMessage = new Message {Content = message.Message};
            var userToMessage = new UserToMessage {Message = newMessage, User = owner};
            var chatToMessage = new ChatToMessage {Message = newMessage, Chat = chat};

            await repository.InsertAsync(userToMessage);
            await repository.InsertAsync(chatToMessage);

            return Ok(newMessage.Id);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChat([FromRoute] Guid chatId)
        {
            var chat = await repository.FindByIdAsync<Chat>(chatId);
            chat.UserToChats.ForEach(async (uc) =>
            {
                await chatHub.Clients.Group(uc.UserId.ToString()).DeleteChat();
                await chatHub.Clients.Group(uc.UserId.ToString()).DeleteChat();
            });
            return NoContent();
        }
    }
}