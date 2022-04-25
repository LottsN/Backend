namespace lab2.Models.DTO
{
    public class GetTopicChildDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? parentId { get; set; }
        public GetTopicChildDTO(int id, string name, int? parentId = null)
        {
            this.id = id;
            this.name = name;
            this.parentId = parentId;
        }
    }
}
