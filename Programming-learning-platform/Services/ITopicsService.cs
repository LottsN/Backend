using lab2.Models;
using lab2.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

namespace lab2.Services
{
    public interface ITopicsService
    {
        JsonResult GetAllTopics(string? name, int? parent);
        Task Add(PostTopicDTO model);
        JsonResult GetOneTopic(int id);
        Task PatchTopic(PostTopicDTO model, int topicId);
        Task DeleteTopic(int id);
        JsonResult GetTopicChilds(int id);
        Task PostTopicChilds(int id, int[] childs);
        Task DeleteTopicChilds(int[] childs);
        bool IsTopicExist(int? id);
    }
    public class TopicsService : ITopicsService
    {
        
        private readonly DatabaseContext _context;

        public TopicsService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task Add(PostTopicDTO model)
        {
            await _context.Topics.AddAsync(new Topic
            (
                model.name,
                model.parentId
            ));
            await _context.SaveChangesAsync();
        }

        public JsonResult GetAllTopics(string? name, int? parent)
        {
            Topic[]? topics = null;
            if (name != null && parent != null)
            {
                topics = _context.Topics.Where(x => x.name == name && x.parentId == parent).ToArray();
            }
            else if (name == null && parent != null)
            {
                topics = _context.Topics.Where(x => x.parentId == parent).ToArray();
            }
            else if (name != null && parent == null)
            {
                topics = _context.Topics.Where(x => x.name == name).ToArray();
            }
            else
            {
                topics = _context.Topics.ToArray(); 
            }

            if (topics == null)
            {
                return null;
            }

            var response = new List<GetTopicDTO>();

            for (int i = 0; i < topics.Length; i++)
            {
                var tmp = new GetTopicDTO(
                    topics[i].id,
                    topics[i].name,
                    GetTopicChilds(topics[i].id).Value,
                    topics[i].parentId
                    );
                response.Add(tmp);
            }

            return new JsonResult(response);
        }
        
        public JsonResult GetOneTopic(int id)
        {
            var topic = _context.Topics.FirstOrDefault(x => x.id == id);

            if (topic == null)
            {
                return null;
            }

            var response = new
            {
                id = topic.id,
                name = topic.name,
                parentId = topic.parentId,
                childs = GetTopicChilds(id).Value,
            };

            return new JsonResult(response);
        }

        public async Task PatchTopic(PostTopicDTO model, int topicId)
        {
            var topic = _context.Topics.FirstOrDefault(x => x.id == topicId);

            topic.name = model.name;
            topic.parentId = model.parentId;

            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTopic(int id)
        {
            var topic = _context.Topics.FirstOrDefault(x => x.id == id);

            var topicsChilds = _context.Topics.Where(x => x.parentId == id).ToArray();

            for (int i = 0; i < topicsChilds.Length; i++)
            {
                await DeleteTopic(topicsChilds[i].id);
            }

            var tasks = _context.Tasks.Where(x => x.topicId == id).ToArray();
            for (int i = 0; i < tasks.Length; i++)
            {
                //delete task input file
                if (tasks[i].input != null)
                {
                    if (File.Exists(tasks[i].input))
                    {
                        File.Delete(tasks[i].input);
                    }
                }

                //delete task output file
                if (tasks[i].output != null)
                {
                    if (File.Exists(tasks[i].output))
                    {
                        File.Delete(tasks[i].output);
                    }
                }
                //delete task
                _context.Tasks.Remove(tasks[i]);
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
        }

        public JsonResult GetTopicChilds(int id)
        {
            var topicChilds = _context.Topics.Where(x => x.parentId == id).Select(x => new GetTopicChildDTO(
                x.id,
                x.name,
                x.parentId
                )).ToArray();

            if (topicChilds == null)
            {
                return null;
            }
            
            return new JsonResult(topicChilds);
        }

        public async Task PostTopicChilds(int id, int[] childs)
        {
            Topic? topic = null;
            for (int i = 0; i < childs.Length; i++)
            {
                if (IsTopicExist(childs[i]))
                {
                    topic = _context.Topics.FirstOrDefault(x => x.id == childs[i]);
                    topic.parentId = id;
                    _context.Topics.Update(topic);
                } 
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTopicChilds(int[] childs)
        {
            Topic? topic = null;
            for (int i = 0; i < childs.Length; i++)
            {
                if (IsTopicExist(childs[i]))
                {
                    topic = _context.Topics.FirstOrDefault(x => x.id == childs[i]);
                    topic.parentId = null;
                    _context.Topics.Update(topic);
                }
            }
            await _context.SaveChangesAsync();
        }

        public bool IsTopicExist(int? id)
        {
            if (id == null)
            {
                return false;
            }

            var topic = _context.Topics.FirstOrDefault(x => x.id == id);

            if (topic == null)
            {
                return false;
            }

            return true;
        }
    }
}
