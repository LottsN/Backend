using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace lab2
{
    [Index(nameof(username), IsUnique = true)]
    public class User
    {
        [Key]
        public int userId { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int roleId { get; set; }
        public string? name { get; set; }
        public string? surname { get; set; }

        
        public User(string username, string password, string name = null, string surname = null)
        {
            this.username = username;
            this.password = password;
            this.roleId = 1;
            this.name = name;
            this.surname = surname;
        }
        
    }
}
