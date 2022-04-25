using System.ComponentModel.DataAnnotations;

namespace lab2.Models
{
    public class BlacklistToken
    {
        [Key]
        public string value { get; set; }
        public BlacklistToken(string value)
        {
            this.value = value;
        }
    }
}
