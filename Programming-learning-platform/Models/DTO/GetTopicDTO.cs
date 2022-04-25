using Microsoft.AspNetCore.Mvc;

namespace lab2.Models.DTO
{
    public class GetTopicDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? parentId { get; set; }
        public object? childs { get; set; }
        public GetTopicDTO(int id, string name, object? childs = null, int? parentId = null)
        {
            this.id = id;
            this.name = name;
            this.childs = childs;
            this.parentId = parentId;
        }
    }
}
