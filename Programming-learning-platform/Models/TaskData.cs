using System.ComponentModel.DataAnnotations;

namespace lab2.Models
{
    public class TaskData
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public int topicId { get; set; }
        public string? description { get; set; }
        public int? price { get; set; }
        public bool isDraft { get; set; }
        public string? input { get; set; }
        public string? output { get; set; }
        public TaskData(string name, int topicId, string? description = "", int? price = 0, bool isDraft = false, string input = null, string output = null)
        { 
            this.id = id;
            this.name = name;
            this.topicId = topicId;
            this.description = description;
            this.price = price;
            this.isDraft = isDraft;
            this.input = input;
            this.output = output;
        }
    }
}
