using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace lab2.Models
{
    [Index(nameof(name), IsUnique = true)]
    public class Role
    {
        [Key]
        public int roleId { get; set; }
        public string name { get; set; }
        public Role(int roleId, string name)
        {
            this.roleId = roleId;
            this.name = name;
        }
    }
}
