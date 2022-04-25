using lab2.Models.DTO;
using lab2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace lab2.Controllers
{
    [Route("[controller]", Order = 4)]
    [ApiController]
    public class solutionsController : ControllerBase
    {
        private ISolutionsService _solutionsService;
        private ITasksService _tasksService;
        private ITokenService _tokenService;
        public solutionsController(ISolutionsService solutions, ITasksService tasks, ITokenService tokens)
        {
            _solutionsService = solutions;
            _tasksService = tasks;
            _tokenService = tokens;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSolutions([FromQuery(Name = "task")] int? task = null, [FromQuery(Name = "user")] int? user = null)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            var tasks = _solutionsService.GetAllSolutions(task, user);
            StatusCode(200, new { message = "OK" });
            return tasks;
        }

        [HttpPost("{solutionId}/postmoderation")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostmoderateSolution([FromBody] PostmoderationDTO model, int solutionId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(401, new { message = "Posting postmoderation model is incorrect" });
            }
            try
            {
                if (!_solutionsService.IsSolutionExist(solutionId))
                {
                    return StatusCode(404, new { message = "Solution with such solutionId is not exist" });
                }
                List<string> allowedVerdicts = new List<string>(new string[] {"Pending", "OK", "Denied"});
                var match = allowedVerdicts.FirstOrDefault(x => x.Contains(model.verdict));
                if (match == null || string.IsNullOrEmpty(model.verdict))
                {
                    return StatusCode(400, new { message = "Allowed only: Pending, OK, Denied verdicts" });
                }
                await _solutionsService.PostmoderateSolution(model, solutionId);
                return _tasksService.GetAllFullTasks();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in posting new task" });
            }
        }
        /*
        [HttpPost("/tasks/{taskId}/solution", Name = "PostTaskSolutionById")]
        public string PostSolution(int taskId)
        {
            return $"This is POST /tasks/{taskId}/solution";
        }

        [HttpGet("/solutions", Name = "GetSolutions")]
        public string Get()
        {
            return "This is GET /solutions";
        }

        [HttpPost("/solutions/{solutionId}/postmoderation", Name = "PostSolutionPostmoderation")]
        public string Post(int solutionId)
        {
            return $"This is POST /solutions{solutionId}/postmoderation";
        }
        */
    }
}
