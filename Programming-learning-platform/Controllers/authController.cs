using lab2.Models;
using lab2.Models.DTO;
using lab2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace lab2.Controllers
{
    [ApiController]
    public class authController : ControllerBase
    {

        private IAuthService _authService;
        private IUsersService _usersService;
        private ITokenService _tokenService;
        //private ITokenManager _tokenManager;
        public authController(IAuthService auth, IUsersService users, ITokenService tokens)
        {
            _authService = auth;
            _usersService = users;
            _tokenService = tokens;
            //_tokenManager = tokens;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(401, new { message = "User model is incorrect" });
                }
                try
                {
                    await _usersService.Add(model);
                    var tmp = new LoginDTO(
                        model.username,
                        model.password
                        );
                    StatusCode(200, "OK");
                    return await Login(tmp);
                }
                catch (Exception ex)
                {
                    return StatusCode(400, new { message = "User with such username are exist" });
                }
            }
            else
            {
                return StatusCode(400, new { message = "You are have already registred in" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {   if (!User.Identity.IsAuthenticated)
            {
                try
                {
                    var result = _authService.login(model);
                    if (result == null)
                    {
                        return StatusCode(401, new { message = "Invalid username or password" });
                    }
                    StatusCode(200, "OK. User logged in.");
                    return result;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Something went wrong in login" });
                }
            }
            else
            {
                return StatusCode(400, new { message = "You are have already logged in" });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {

            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }

            var tokenToBlacklist = new BlacklistToken(_bearer_token);
            await _tokenService.AddToBlacklist(tokenToBlacklist);
            return StatusCode(200, new { message = $"You logged out" });
        }
    }
}
