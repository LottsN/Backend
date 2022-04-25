using lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab2.Services
{
    public interface IRolesService
    {
        JsonResult GetAllRoles();
        JsonResult GetOneRole(int roleId);
        bool IsRoleExist(int roleId);
    }

    public class RolesService : IRolesService
    {
        private readonly DatabaseContext _context;

        public RolesService(DatabaseContext context)
        {
            _context = context;
        }

        public JsonResult GetAllRoles()
        {
            var roles = _context.Roles.ToArray();
            return new JsonResult(roles);
        }

        public JsonResult GetOneRole(int roleId)
        {
            var role = _context.Roles.FirstOrDefault(x => x.roleId == roleId);
            return new JsonResult(role);
        }

        public bool IsRoleExist(int roleId)
        {
            if ( _context.Roles.FirstOrDefault(x => x.roleId == roleId) == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
