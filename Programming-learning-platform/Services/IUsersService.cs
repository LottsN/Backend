using lab2.Models;
using lab2.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace lab2.Services
{
    public interface IUsersService
    {
        //декларируем функции
        Task Add(RegisterDTO model);
        Task PatchUser(PatchUserDTO model, int userId);
        Task DeleteUser(int userId);
        Task PostRole(PostRoleDTO model, int userId);
        JsonResult GetAllUsers();
        JsonResult GetOneUser(int userId);
        bool CheckIfAdmin(string username);
        bool CheckIfOwner(int userId, string username);
        bool IsUserExist(int userId);
    }

    public class UsersService : IUsersService
    {
        //пробрасываем контекст в сервис
        private readonly DatabaseContext _context;

        public UsersService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task Add(RegisterDTO model)
        {
            await _context.Users.AddAsync(new User
            (
                model.username,
                model.password,
                model.name,
                model.surname
            ));
            await _context.SaveChangesAsync();
        }

        public async Task PatchUser(PatchUserDTO model, int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.userId == userId);
            user.password = model.password;
            user.name = model.name;
            user.surname = model.surname;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.userId == userId);
            _context.Users.Remove(user);
            /*
            var solutions = _context.Solutions.Where(x => x.authorId == userId).ToArray();
            for (int i = 0; i < solutions.Length; i++)
            {
                _context.Solutions.Remove(solutions[i]);
            }
            */
            await _context.SaveChangesAsync();
        }

        public async Task PostRole(PostRoleDTO model, int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.userId == userId);
            user.roleId = model.roleId;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public JsonResult GetAllUsers()
        {
            var users = _context.Users.Select(x => new GetUserDTO(
                x.userId,
                x.username,
                x.roleId
                )
            ).ToArray();
            return new JsonResult(users);
        }

        public JsonResult GetOneUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.userId == userId);

            if (user == null)
            {
                return null;
            }

            var response = new
            {
                userId = user.userId,
                username = user.username,
                roleId = user.roleId,
                name = user.name,
                surname = user.surname,
            };

            return new JsonResult(response);
        }

        public bool IsUserExist(int userId)
        {
            var result = GetOneUser(userId);
            if (result == null)
            {
                return false;
            }
            return true;
        }

        public bool CheckIfAdmin(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.username == username);

            if (user == null)
            {
                return false;
            }

            if (user.roleId == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckIfOwner(int userId, string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.username == username);

            if (user == null)
            {
                return false;
            }

            if (user.userId == userId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
 