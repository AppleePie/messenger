using System;
using System.Collections.Generic;
using System.IO;
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

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetUsers() =>
            Ok(repository.GetUsers().Select(u => mapper.Map<UserToSendDto>(u)).ToList());

        [HttpGet("{id:guid}", Name = nameof(GetUserByIdAsync))]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id)
        {
            var user = await repository.FindByIdAsync<User>(id);
            if (user is null)
                return NotFound();
            var userToSend = mapper.Map<UserToSendDto>(user);

            return Ok(userToSend);
        }

        [HttpGet("check")]
        public async Task<IActionResult> GetUserByName([FromQuery] string login, [FromQuery] string password)
        {
            var user = await repository.FindByLoginAsync(login);

            if (user is null)
                return NotFound();
            if (user.Password != password)
                return BadRequest();
            
            return Ok(user.Id);
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
            var user = await repository.FindByIdAsync<User>(id);
            var removingIds = new List<Guid>();
            foreach (var chat in user.UserToChats.Select(userToChat => userToChat.Chat))
            {
                foreach (var interlocutor in chat.UserToChats.Select(uc => uc.User).Where(u => u.Id != id))
                {
                    var idStart = interlocutor.RelationsWithChats.IndexOf(chat.Id.ToString(), StringComparison.Ordinal);
                    interlocutor.RelationsWithChats = interlocutor.RelationsWithChats.Remove(idStart, chat.Id.ToString().Length);
                    removingIds.Add(chat.Id);
                    await repository.UpdateAsync(interlocutor);
                }
            }
            
            removingIds.ForEach(async (i) => await repository.DeleteAsync<Chat>(i));
            
            await repository.DeleteAsync<User>(id);
            return NoContent();
        }

        [HttpGet("{userId:guid}/avatar")]
        public async Task<IActionResult> GetUserAvatarAsync([FromRoute] Guid userId)
        {
            await using var fileStream =
                new FileStream(Path.Combine(uploadDirectory, userId.ToString()), FileMode.Open);
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer);
            return Ok($"data:image/*;base64,{Convert.ToBase64String(buffer)}");
        }

        [HttpPost("{id:guid}/avatar")]
        public async Task<IActionResult> AddUserAvatar([FromRoute] Guid id, [FromBody] IFormFileCollection uploads)
        {
            if (uploads.Count != 1)
                return BadRequest("Avatar file collection should contains only one element!");

            var uploadedFile = uploads[0];
            var path = Path.Combine(uploadDirectory, id.ToString());
            await using var fileStream = new FileStream(path, FileMode.Create);
            await uploadedFile.CopyToAsync(fileStream);

            return NoContent();
        }
    }
}