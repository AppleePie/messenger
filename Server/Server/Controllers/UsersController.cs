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
        private readonly IUserRepository userRepository;
        public UsersController(IUserRepository userRepository) => this.userRepository = userRepository;

        [HttpGet("{id:guid}", Name = nameof(GetUserByIdAsync))]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id) =>
            Ok(await userRepository.FindByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserEntity userEntity)
        {
            if (userEntity is null)
                return BadRequest("User body should not be null or empty!");

            if (await userRepository.FindByLoginAsync(userEntity.Login) != null)
                return Conflict("User with this username already exists!");

            var insertedUserId = await userRepository.InsertAsync(userEntity);
            return CreatedAtRoute(nameof(GetUserByIdAsync), new {id = insertedUserId}, insertedUserId);
        }

        [HttpPost("{id:guid}/avatar")]
        public async Task<IActionResult> AddAvatar([FromRoute] Guid id, IFormFileCollection uploads)
        {
            if (uploads.Count != 1)
                return BadRequest("Avatar file collection should contains only one element!");

            var uploadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");
            if (!Directory.Exists(uploadDirectory)) 
                Directory.CreateDirectory(uploadDirectory);

            var uploadedFile = uploads[0];
            var path = Path.Combine(uploadDirectory, uploadedFile.FileName);
            await using var fileStream = new FileStream(path, FileMode.Create);
            await uploadedFile.CopyToAsync(fileStream);

            await userRepository.ChangeAvatarAsync(id, uploadedFile.FileName);
            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUserById([FromRoute] Guid id) =>
            Ok(await userRepository.DeleteAsync(id));
    }
}