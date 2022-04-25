using lab2.Models;
using lab2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace lab2.Controllers
{
    [Route("[controller]", Order = 4)]
    [ApiController]
    public class rolesController : ControllerBase
    {
        private IRolesService _rolesService;
        private ITokenService _tokenService;
        public rolesController(IRolesService roles, ITokenService tokens)
        {
            _rolesService = roles;
            _tokenService = tokens;
        }

        [HttpGet(Name = "GetRoles")]
        [Authorize]
        public async Task<IActionResult> GetAllRoles()
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            return _rolesService.GetAllRoles();
        }

        [HttpGet("{roleId}", Name = "GetOneRole")]
        [Authorize]
        public async Task<IActionResult> GetOneRole(int roleId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            return _rolesService.GetOneRole(roleId);
        }
    }
}
