using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
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
        private readonly IUserRepository userRepository;
        private readonly string uploadDirectory;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;

            var uploadDirectoryName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UploadDirectoryName);
            if (!Directory.Exists(uploadDirectoryName))
                Directory.CreateDirectory(uploadDirectoryName);
            uploadDirectory = uploadDirectoryName;
        }

        [HttpGet("{id:guid}", Name = nameof(GetUserByIdAsync))]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id)
        {
            var user = await userRepository.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserDto userEntity)
        {
            if (userEntity is null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            if (await userRepository.FindByLoginAsync(userEntity.Login) != null)
                return Conflict("User with this username already exists!");

            var insertedUserId = await userRepository.InsertAsync(userEntity);
            return CreatedAtRoute(nameof(GetUserByIdAsync), new {id = insertedUserId}, insertedUserId);
        }

        [HttpGet("{userId:guid}/avatar")]
        public async Task<IActionResult> GetUserAvatarAsync([FromRoute] Guid userId)
        {
            var avatarPath = (await userRepository.FindByIdAsync(userId)).AvatarFilePath;
            await using var fileStream = new FileStream(Path.Combine(uploadDirectory, avatarPath), FileMode.Open);
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

            await userRepository.ChangeAvatarAsync(id);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUserById([FromRoute] Guid id)
        {
            await userRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}