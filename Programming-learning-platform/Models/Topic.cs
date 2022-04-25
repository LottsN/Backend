using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace lab2.Models
{
    public class Topic
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public int? parentId { get; set; }
        public Topic(string name, int? parentId = null )
        {
            this.name = name;
            this.parentId = parentId;
        }
    }
}
