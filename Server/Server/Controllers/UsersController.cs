using System;
using System.IO;
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
        [HttpGet("{id:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id) =>
            Ok(await UserRepository.FindByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] User user)
        {
            if (user is null)
                return BadRequest("User body should not be null or empty!");

            if (await UserRepository.FindByNameAsync(user.Name) != null)
                return Conflict("User with this username already exists!");

            var insertedUserId = await UserRepository.InsertAsync(user);
            return Ok(insertedUserId);
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

            await UserRepository.ChangeUserAvatarAsync(id, uploadedFile.FileName);
            return NoContent();
        }
    }
}