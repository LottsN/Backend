using lab2.Models.DTO;
using lab2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace lab2.Controllers
{
    [ApiController]
    [Route("[controller]", Order = 1)]
    public class usersController : ControllerBase
    {
        //пробросили сервис в контроллер
        private IUsersService _usersService;
        private IRolesService _rolesService;
        private ITokenService _tokenService;
        public usersController(IUsersService users, IRolesService roles, ITokenService tokens)
        {
            _usersService = users;
            _rolesService = roles;
            _tokenService = tokens;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            return _usersService.GetAllUsers();
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetOneUser(int userId)
        {

            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }

            if (_usersService.CheckIfOwner(userId, User.Identity.Name) || _usersService.CheckIfAdmin(User.Identity.Name))
            {
                try
                {
                    if (!_usersService.IsUserExist(userId))
                    {
                        return StatusCode(404, new { message = "User not found" });
                    }
                    StatusCode(200, "OK");
                    return _usersService.GetOneUser(userId);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Something went wrong in get one user" });
                }
            }
            else
            {
                return StatusCode(403, new { message = "You have no permission to do that" });
            }
        }

        [HttpPatch("{userId}")]
        [Authorize]
        public async Task<IActionResult> PatchUser([FromBody] PatchUserDTO model, int userId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }

            if (_usersService.CheckIfOwner(userId, User.Identity.Name) || _usersService.CheckIfAdmin(User.Identity.Name))
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(400, new { message = "Patch model is incorrect" });
                }
                try
                {
                    if (!_usersService.IsUserExist(userId))
                    {
                        return StatusCode(404, new { message = "User not found" });
                    }
                    await _usersService.PatchUser(model, userId);
                    StatusCode(200, "OK");
                    return _usersService.GetOneUser(userId);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Something went wrong in patch user" });
                }
            }
            else
            {
                return StatusCode(403, new { message = "You have no permission to do that" });
            }
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }

            try
            {
                if (!_usersService.IsUserExist(userId))
                {
                    return StatusCode(404, new { message = "User not found" });
                }
                await _usersService.DeleteUser(userId);
                return StatusCode(200, new { message = "OK" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in delete user" });
            }
        }

        [HttpPost("{userId}/role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostRole([FromBody] PostRoleDTO model, int userId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(401, new { message = "Post role model is incorrect" });
            }
            try
            {
                if (!_usersService.IsUserExist(userId))
                {
                    return StatusCode(404, new { message = "User not found" });
                }

                if (_rolesService.IsRoleExist(model.roleId))
                {
                    await _usersService.PostRole(model, userId);
                    return StatusCode(200, new { message = "OK" });
                }
                else
                {
                    return StatusCode(404, new { message = "Role with such id doesn't exist" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in posting user's role" });
            }
        }
    }
}
