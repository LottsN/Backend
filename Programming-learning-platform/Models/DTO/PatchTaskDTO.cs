namespace lab2.Models.DTO
{
    public class PatchTaskDTO
    {
        public string name { get; set; }
        public int topicId { get; set; }
        public string? description { get; set; }
        public int? price { get; set; }
        public PatchTaskDTO(string name, int topicId, string? description = "", int? price = 0)
        {
            this.name = name;
            this.topicId = topicId;
            this.description = description;
            this.price = price;
        }
    }
}
