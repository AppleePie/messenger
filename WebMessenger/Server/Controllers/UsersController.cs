using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DatabaseWorkers;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/users/")]
    public class UsersController : Controller
    {
        private const string UploadDirectoryName = "Files";
        private readonly Repository repository;
        private readonly IMapper mapper;
        private readonly string uploadDirectory;

        public UsersController(Repository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;

            var uploadDirectoryName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UploadDirectoryName);
            if (!Directory.Exists(uploadDirectoryName))
                Directory.CreateDirectory(uploadDirectoryName);
            uploadDirectory = uploadDirectoryName;
        }

        [HttpGet("{id:guid}", Name = nameof(GetUserByIdAsync))]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id)
        {
            var user = await repository.FindByIdAsync<User>(id);
            if (user is null)
                return NotFound();
            var userToSend = new UserToSendDto
            {
                Id = user.Id,
                Login = user.Login,
                Chats = user.UserToChats.Select(c => new ChatToSendDto()
                {
                    Id = c.ChatId,
                    Interlocutor = c.Chat.UserToChats.First(u => u.UserId != user.Id).UserId
                }).ToList()
            };

            return Ok(userToSend);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserToCreateDto user)
        {
            if (user is null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            if (await repository.FindByLoginAsync(user.Login) != null)
                return Conflict("User with this username already exists!");

            var userDto = mapper.Map<User>(user);

            var insertedUser = await repository.InsertAsync(userDto);
            return CreatedAtRoute(nameof(GetUserByIdAsync), new {id = insertedUser.Id}, insertedUser.Id);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUserById([FromRoute] Guid id)
        {
            await repository.DeleteAsync<User>(id);
            return NoContent();
        }

        [HttpGet("{userId:guid}/avatar")]
        public async Task<IActionResult> GetUserAvatarAsync([FromRoute] Guid userId)
        {
            await using var fileStream = new FileStream(Path.Combine(uploadDirectory, userId.ToString()), FileMode.Open);
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer);
            return Ok($"data:image/*;base64,{Convert.ToBase64String(buffer)}");
        }

        [HttpPost("{id:guid}/avatar")]
        public async Task<IActionResult> AddUserAvatar([FromRoute] Guid id, IFormFileCollection uploads)
        {
            if (uploads.Count != 1)
                return BadRequest("Avatar file collection should contains only one element!");

            var uploadedFile = uploads[0];
            var path = Path.Combine(uploadDirectory, id.ToString());
            await using var fileStream = new FileStream(path, FileMode.Create);
            await uploadedFile.CopyToAsync(fileStream);
            
            return NoContent();
        }

        [HttpPost("users/new-chat")]
        public async Task<IActionResult> CreateChat([FromBody] ChatToCreateDto newChat)
        {
            if (newChat is null)
                return BadRequest();
            var initiator = await repository.FindByIdAsync<User>(newChat.InitiatorId);
            var interlocutor = await repository.FindByLoginAsync(newChat.InterlocutorName);
            var chat = new Chat();
            var chat1 = new UserToChat { User = initiator, Chat = chat};
            var chat2 = new UserToChat {User = interlocutor, Chat = chat};
            await repository.InsertAsync(chat1);
            await repository.InsertAsync(chat2);
            
            initiator.RelationsWithChats += $"{chat.Id}{Models.User.Delimiter}";
            interlocutor.RelationsWithChats += $"{chat.Id}{Models.User.Delimiter}";
            
            await repository.UpdateAsync(initiator);
            await repository.UpdateAsync(interlocutor);

            return NoContent();
        }
    }
}