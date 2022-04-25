using lab2.Models;
using lab2.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace lab2.Services
{
    public interface ITasksService
    {
        JsonResult GetAllTasks(string? name, int? topic);
        JsonResult GetAllFullTasks();
        Task PostTask(PostTaskDTO model);
        JsonResult GetOneTask(int id);
        Task PatchTask(PatchTaskDTO model, int id);
        Task DeleteTask(int id);
        JsonResult GetTaskInput(int id);
        Task AddTaskInput(int id, string? path);
        Task DeleteTaskInput(int id);
        JsonResult GetTaskOutput(int id);
        Task AddTaskOutput(int id, string? path);
        Task DeleteTaskOutput(int id);
        Task<JsonResult> PostSolution(PostSolutionDTO model, int taskId, string username);
        bool IsTaskExist(int? id);
    }
    public class TasksService : ITasksService
    {
        private readonly DatabaseContext _context;

        public TasksService(DatabaseContext context)
        {
            _context = context;
        }

        public JsonResult GetAllTasks(string? name, int? topic)
        {
            TaskData[]? tasks = null;
            if (name != null && topic != null)
            {
                tasks = _context.Tasks.Where(x => x.name == name && x.topicId == topic).ToArray();
            }
            else if (name == null && topic != null)
            {
                tasks = _context.Tasks.Where(x => x.topicId == topic).ToArray();
            }
            else if (name != null && topic == null)
            {
                tasks = _context.Tasks.Where(x => x.name == name).ToArray();
            }
            else
            {
                tasks = _context.Tasks.ToArray();
            }

            if (tasks == null)
            {
                return null;
            }

            var response = new List<GetTaskDTOShort>();

            for (int i = 0; i < tasks.Length; i++)
            {
                var tmp = new GetTaskDTOShort(
                    tasks[i].id,
                    tasks[i].name,
                    tasks[i].topicId
                    );
                response.Add(tmp);
            }

            return new JsonResult(response);
        }

        public JsonResult GetAllFullTasks()
        {
            var tasks = _context.Tasks.ToArray();

            if (tasks == null)
            {
                return null;
            }

            var response = new List<GetTaskDTOLong>();

            for (int i = 0; i < tasks.Length; i++)
            {
                var tmp = new GetTaskDTOLong(
                    tasks[i].id,
                    tasks[i].name,
                    tasks[i].topicId,
                    tasks[i].description,
                    tasks[i].price,
                    tasks[i].isDraft
                    );
                response.Add(tmp);
            }

            return new JsonResult(response);
        }

        public async Task PostTask(PostTaskDTO model)
        {
            await _context.Tasks.AddAsync(new TaskData
            (
                model.name,
                model.topicId,
                model.description,
                model.price
            ));
            await _context.SaveChangesAsync();
        }

        public JsonResult GetOneTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.id == id);

            if (task == null)
            {
                return null;
            }

            var response = new
            {
                id = task.id,
                name = task.name,
                topicId = task.topicId,
                description = task.description,
                price = task.price,
                isDraft = task.isDraft,
            };

            return new JsonResult(response);
        }

        public async Task PatchTask(PatchTaskDTO model, int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.id == id);

            task.name = model.name;
            task.topicId = model.topicId;
            task.description = model.description;
            task.price = model.price;

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.id == id);
            _context.Tasks.Remove(task);

            /* добавить обнуление решений при удалении
            var topics = _context.Topics.Where(x => x.parentId == id).ToArray();
            for (int i = 0; i < topics.Length; i++)
            {
                topics[i].parentId = null;
                _context.Topics.Update(topics[i]);
            }
            */
            await _context.SaveChangesAsync();
        }

        public JsonResult GetTaskInput(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.id == id);

            var response = new
            {
                input = task.input,
            };

            return new JsonResult(response);
        }

        public async Task AddTaskInput(int id, string? path)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.id == id);
            task.input = path;
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskInput(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.id == id);
            if (task.input != null)
            {
                task.input = null;
                if (File.Exists(task.input))
                {
                    File.Delete(task.input);
                }
            }
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public JsonResult GetTaskOutput(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.id == id);

            var response = new
            {
                output = task.output,
            };

            return new JsonResult(response);
        }

        public async Task AddTaskOutput(int id, string? path)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.id == id);
            task.output = path;
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskOutput(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.id == id);
            if (task.output != null)
            {
                task.output = null;
                if (File.Exists(task.output))
                {
                    File.Delete(task.output);
                }
            }
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task<JsonResult> PostSolution(PostSolutionDTO model, int taskId, string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.username == username);
            var solution = new Solution
            (
                model.sourceCode,
                model.programmingLanguage,
                user.userId,
                taskId
            );
            await _context.Solutions.AddAsync(solution);
            await _context.SaveChangesAsync();
            var response = new
            {
                id = solution.id,
                sourceCode = solution.sourceCode,
                programmingLanguage = solution.programmingLanguage,
                verdict = solution.verdict,
                authorId = solution.authorId,
                taskId = solution.taskId
            };

            return new JsonResult(response);
        }

        public bool IsTaskExist(int? id)
        {
            if (id == null)
            {
                return false;
            }

            var task = _context.Tasks.FirstOrDefault(x => x.id == id);

            if (task == null)
            {
                return false;
            }

            return true;
        }
    }
}
