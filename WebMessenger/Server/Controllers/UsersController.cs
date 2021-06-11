using System;
using System.Collections.Generic;
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

        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid userId, [FromBody] UserToUpdateDto userToUpdate)
        {
            if (userToUpdate is null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var user = await repository.FindByIdAsync<User>(userId);
            if (user == null)
                return Conflict("User with this username not exists!");
            if (!string.IsNullOrEmpty(userToUpdate.Login) && await repository.FindByLoginAsync(userToUpdate.Login) != null)
                return Conflict("User with this username already exists");
            user.Login = !string.IsNullOrEmpty(userToUpdate.Login) ? userToUpdate.Login : user.Login;
            user.Password = !string.IsNullOrEmpty(userToUpdate.Password) ? userToUpdate.Password : user.Password;
            await repository.UpdateAsync(user);
            return Ok(user.Id);
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
            var filePath = Path.Combine(uploadDirectory, userId.ToString());
            if (!System.IO.File.Exists(filePath))
                return NotFound();
            
            await using var fileStream = new FileStream(filePath, FileMode.Open);
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer);
            return Ok($"data:image/*;base64,{Convert.ToBase64String(buffer)}");
        }

        [HttpPost("{id:guid}/avatar")]
        public async Task<IActionResult> ChangeUserAvatar([FromRoute] Guid id, [FromForm] IFormFileCollection uploads)
        {
            if (uploads.Count != 1)
                return BadRequest("Avatar file collection should contains only one element!");

            var uploadedFile = uploads[0];
            var filePath = id.ToString();
            var path = Path.Combine(uploadDirectory, filePath);
            await using var fileStream = new FileStream(path, FileMode.OpenOrCreate);
            await uploadedFile.CopyToAsync(fileStream);

            return NoContent();
        }
    }
}