using lab2.Models;
using lab2.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace lab2.Services
{
    public interface IAuthService
    {
        JsonResult login([FromBody] LoginDTO model);
    }

    public class AuthService : IAuthService
    {
        private readonly DatabaseContext _context;

        public AuthService(DatabaseContext context)
        {
            _context = context;
        }

        public JsonResult login([FromBody] LoginDTO model)
        {
            var identity = GetIdentity(model.username, model.password);

            if (identity == null)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.AddHours(JwtConfigurations.Lifetime),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                token = encodedJwt,
            };

            return new JsonResult(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.username == username && x.password == password);

            if (user == null)
            {
                return null;
            }

            //roles dictionary
            var roles = new Dictionary<int, string>(){
                {1, "customer"},
                {2, "moderator"},
                {3, "admin"}
            };

            // Claims описывают набор базовых данных для авторизованного пользователя
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, roles[ (user.roleId - 1) % roles.Count + 1])
            };

            //Claims identity и будет являться полезной нагрузкой в JWT токене, которая будет проверяться стандартным атрибутом Authorize
            var claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
