namespace lab2.Models.DTO
{
    public class GetTaskDTOShort
    {
        public int id { get; set; }
        public string name { get; set; }
        public int topicId { get; set; }
        public GetTaskDTOShort(int id, string name, int topicId)
        {
            this.id = id;
            this.name = name;
            this.topicId = topicId;
        }
    }
}
