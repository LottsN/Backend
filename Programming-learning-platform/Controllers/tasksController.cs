using lab2.Models.DTO;
using lab2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace lab2.Controllers
{
    [Route("[controller]", Order = 3)]
    [ApiController]
    public class tasksController : ControllerBase
    {
        private ITasksService _tasksService;
        private ITopicsService _topicsService;
        private IWebHostEnvironment _appEnvironment;
        private ITokenService _tokenService;
        public tasksController(ITasksService tasks, ITopicsService topics, IWebHostEnvironment appEnvironment, ITokenService tokens)
        {
            _tasksService = tasks;
            _topicsService = topics;
            _appEnvironment = appEnvironment;
            _tokenService = tokens;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromQuery(Name = "name")] string? name = null, [FromQuery(Name = "topic")] int? topic = null)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            var tasks = _tasksService.GetAllTasks(name, topic);
            StatusCode(200, new { message = "OK" });
            return tasks;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostTask([FromBody] PostTaskDTO model)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(401, new { message = "Post task model is incorrect" });
            }
            try
            {
                if (!_topicsService.IsTopicExist(model.topicId))
                {
                    return StatusCode(404, new { message = "Topic with such topicId is not exist" });
                }
                await _tasksService.PostTask(model);
                return _tasksService.GetAllFullTasks();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in posting new task" });
            }
        }

        [Authorize]
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetOneTask(int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            try
            {
                if (!_tasksService.IsTaskExist(taskId))
                {
                    return StatusCode(404, new { message = "Task is not exist" });
                }
                var task = _tasksService.GetOneTask(taskId);
                StatusCode(200, new { message = "OK" });
                return task;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in getting one task" });
            }
        }

        [HttpPatch("{taskId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PatchTask([FromBody] PatchTaskDTO model, int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(401, new { message = "Patch model is incorrect" });
            }
            try
            {
                if (!_tasksService.IsTaskExist(taskId))
                {
                    return StatusCode(404, new { message = "Task not found" });
                }
                await _tasksService.PatchTask(model, taskId);
                StatusCode(200, "OK");
                return _tasksService.GetOneTask(taskId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in patch task" });
            }
        }

        [HttpDelete("{taskId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            try
            {
                if (!_tasksService.IsTaskExist(taskId))
                {
                    return StatusCode(404, new { message = "Task not found" });
                }
                await _tasksService.DeleteTask(taskId);
                return StatusCode(200, new { message = "OK" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in deleting task" });
            }
        }

        [HttpGet("{taskId}/input")]
        [Authorize]
        public async Task<IActionResult> GetTaskInput(int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!_tasksService.IsTaskExist(taskId))
            {
                return StatusCode(404, new { message = "Task not found" });
            }
            return _tasksService.GetTaskInput(taskId);
        }

        [HttpPost("{taskId}/input")]
        [Authorize]
        public async Task<IActionResult> AddTaskInput(IFormFile input, int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!_tasksService.IsTaskExist(taskId))
            {
                return StatusCode(404, new { message = "Task not found" });
            }
            if (input != null)
            {
                var supportedTypes = new[] { "txt", "doc", "docx" };
                var fileExt = System.IO.Path.GetExtension(input.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    return StatusCode(400, new { message = "Invalid file type" });
                }
                string path = "./Uploads/" + "input_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + input.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await input.CopyToAsync(fileStream);
                }

                await _tasksService.AddTaskInput(taskId, path);
                return _tasksService.GetOneTask(taskId);
            }
            return StatusCode(404, new { message = "No file to upload" });
        }


        [HttpDelete("{taskId}/input")]
        [Authorize]
        public async Task<IActionResult> DeleteTaskInput(int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!_tasksService.IsTaskExist(taskId))
            {
                return StatusCode(404, new { message = "Task not found" });
            }
            await _tasksService.DeleteTaskInput(taskId);
            return StatusCode(200, new { message = "OK" });
        }

        [HttpGet("{taskId}/output")]
        [Authorize]
        public async Task<IActionResult> GetTaskOutput(int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!_tasksService.IsTaskExist(taskId))
            {
                return StatusCode(404, new { message = "Task not found" });
            }
            return _tasksService.GetTaskOutput(taskId);
        }

        [HttpPost("{taskId}/output")]
        [Authorize]
        public async Task<IActionResult> AddTaskOutput(IFormFile output, int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!_tasksService.IsTaskExist(taskId))
            {
                return StatusCode(404, new { message = "Task not found" });
            }
            if (output != null)
            {
                var supportedTypes = new[] { "txt", "doc", "docx" };
                var fileExt = System.IO.Path.GetExtension(output.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    return StatusCode(400, new { message = "Invalid file type" });
                }
                string path = "./Uploads/" + "output_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + output.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await output.CopyToAsync(fileStream);
                }

                await _tasksService.AddTaskOutput(taskId, path);
                return _tasksService.GetOneTask(taskId);
            }
            return StatusCode(404, new { message = "No file to upload" });
        }

        [HttpDelete("{taskId}/output")]
        [Authorize]
        public async Task<IActionResult> DeleteTaskOutput(int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!_tasksService.IsTaskExist(taskId))
            {
                return StatusCode(404, new { message = "Task not found" });
            }
            await _tasksService.DeleteTaskOutput(taskId);
            return StatusCode(200, new { message = "OK" });
        }

        [HttpPost("{taskId}/solution")]
        [Authorize]
        public async Task<IActionResult> PostSolution([FromBody] PostSolutionDTO model, int taskId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(401, new { message = "Post solution model is incorrect" });
            }
            try
            {
                if (!_tasksService.IsTaskExist(taskId))
                {
                    return StatusCode(404, new { message = "Task with such taskId is not exist" });
                }
                List<string> allowedLanguages = new List<string>(new string[] { "C++", "C#", "Java", "Pascal", "Python" });
                var match = allowedLanguages.FirstOrDefault(x => x.Contains(model.programmingLanguage));
                if (match == null || string.IsNullOrEmpty(model.programmingLanguage) )
                {
                    return StatusCode(400, new { message = "Allowed only: C++, C#, Java, Pascal, Python languages" });
                }
                return await _tasksService.PostSolution(model, taskId, User.Identity.Name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in posting new task" });
            }
        }

        /*
        [HttpGet(Name = "GetTasks")]
        public string Get()
        {
            return "This is GET /tasks";
        }

        [HttpPost(Name = "PostTasks")]
        public string Post()
        {
            return "This is POST /tasks";
        }

        [HttpGet("{taskId}", Name = "GetTaskById")]
        public string Get(int taskId)
        {
            return $"This is GET /tasks/{taskId}";
        }

        [HttpPatch("{taskId}", Name = "PatchTaskById")]
        public string Patch(int taskId)
        {
            return $"This is PATCH /tasks/{taskId}";
        }

        [HttpDelete("{taskId}", Name = "DeleteTaskById")]
        public string Delete(int taskId)
        {
            return $"This is DELETE /tasks/{taskId}";
        }

        [HttpGet("{taskId}/input", Name = "GetTaskInputById")]
        public string GetInput(int taskId)
        {
            return $"This is GET /tasks/{taskId}/input";
        }

        [HttpPost("{taskId}/input", Name = "PostTaskInputById")]
        public string PostInput(int taskId)
        {
            return $"This is POST /tasks/{taskId}/input";
        }

        [HttpDelete("{taskId}/input", Name = "DeleteTaskInputById")]
        public string DeleteInput(int taskId)
        {
            return $"This is DELETE /tasks/{taskId}/input";
        }

        [HttpGet("{taskId}/output", Name = "GetTaskOutputById")]
        public string GetOutput(int taskId)
        {
            return $"This is GET /tasks/{taskId}/output";
        }

        [HttpPost("{taskId}/output", Name = "PostTaskOutputById")]
        public string PostOutput(int taskId)
        {
            return $"This is POST /tasks/{taskId}/output";
        }

        [HttpDelete("{taskId}/output", Name = "DeleteTaskOutputById")]
        public string DeleteOutput(int taskId)
        {
            return $"This is DELETE /tasks/{taskId}/output";
        }
        */
    }
}
