namespace lab2.Models.DTO
{
    public class GetTaskDTOLong
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? topicId { get; set; }
        public string? description { get; set; }
        public int? price { get; set; }
        public bool isDraft { get; set; }
        public GetTaskDTOLong(int id, string name, int? topicId, string? description = "", int? price = 0, bool isDraft = false)
        {
            this.id = id;
            this.name = name;
            this.topicId = topicId;
            this.description = description;
            this.price = price;
            this.isDraft = isDraft;
        }
    }
}
