namespace lab2.Models.DTO
{
    public class PostTopicDTO
    {
        public string name { get; set; }
        public int? parentId { get; set; }
        public PostTopicDTO(string name, int? parentId)
        {
            this.name = name;
            this.parentId = parentId;
        }
    }
}
