using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.DatabaseWorkers;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/chats/")]
    public class ChatsController : Controller
    {
        private readonly Repository repository;
        private readonly IMapper mapper;
        public ChatsController(Repository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetChatById(Guid id)
        {
            var chat = await repository.FindByIdAsync<Chat>(id);
            if (chat is null)
                return NotFound();

            var chatDto = mapper.Map<ChatToSend>(chat);
            return Ok(chatDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] ChatToCreateDto newChat)
        {
            if (newChat is null)
                return BadRequest();
            var initiator = await repository.FindByIdAsync<User>(newChat.InitiatorId);
            var interlocutor = await repository.FindByLoginAsync(newChat.InterlocutorName);
            var chat = new Chat();
            var chat1 = new UserToChat {User = initiator, Chat = chat};
            var chat2 = new UserToChat {User = interlocutor, Chat = chat};
            await repository.InsertAsync(chat1);
            await repository.InsertAsync(chat2);

            initiator.RelationsWithChats += $"{chat.Id}{Models.User.Delimiter}";
            interlocutor.RelationsWithChats += $"{chat.Id}{Models.User.Delimiter}";

            await repository.UpdateAsync(initiator);
            await repository.UpdateAsync(interlocutor);

            return Ok(chat.Id);
        }

        [HttpDelete("{chatId:guid}")]
        public async Task<IActionResult> DeleteChat([FromRoute] Guid chatId)
        {
            var chat = await repository.FindByIdAsync<Chat>(chatId);
            foreach (var user in chat.UserToChats.Select(uc => uc.User))
            {
                var idStart = user.RelationsWithChats.IndexOf(chatId.ToString(), StringComparison.Ordinal);
                user.RelationsWithChats = user.RelationsWithChats.Remove(idStart, chatId.ToString().Length);
                await repository.UpdateAsync(user);
            }
            
            await repository.DeleteAsync<Chat>(chatId);
            return NoContent();
        }
    }
}