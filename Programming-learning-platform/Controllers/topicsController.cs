using lab2.Models;
using lab2.Models.DTO;
using lab2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace lab2.Controllers
{
    [Route("[controller]", Order = 2)]
    [ApiController]
    public class topicsController : ControllerBase
    {
        private ITopicsService _topicsService;
        private ITokenService _tokenService;
        public topicsController(ITopicsService topics, ITokenService tokens)
        {
            _topicsService = topics;
            _tokenService = tokens;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopics([FromQuery(Name = "name")] string? name = null, [FromQuery(Name = "parent")] int? parent = null)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            var topics = _topicsService.GetAllTopics(name, parent);
            StatusCode(200, new { message = "OK" });
            return topics;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostTopic([FromBody] PostTopicDTO model)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(401, new { message = "Post topic model is incorrect" });
            }
            try
            {
                if (!_topicsService.IsTopicExist(model.parentId))
                {
                    if (model.parentId == null)
                    {
                        await _topicsService.Add(model);
                        return _topicsService.GetAllTopics(null, null);
                    }
                    return StatusCode(404, new { message = "Parent topic is not exist" });
                }
                await _topicsService.Add(model);
                return _topicsService.GetAllTopics(null, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in posting new topic" });
            }
        }

        [Authorize]
        [HttpGet("{topicId}")]
        public async Task<IActionResult> GetOneTopic(int topicId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            try
            {
                if (!_topicsService.IsTopicExist(topicId))
                {
                    return StatusCode(404, new { message = "Topic is not exist" });
                }
                var topic = _topicsService.GetOneTopic(topicId);
                StatusCode(200, new { message = "OK" });
                return topic;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in getting one topic" });
            }
        }

        [HttpPatch("{topicId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PatchTopic([FromBody] PostTopicDTO model, int topicId)
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
                if (!_topicsService.IsTopicExist(topicId))
                {
                    return StatusCode(404, new { message = "Topic not found" });
                }
                await _topicsService.PatchTopic(model, topicId);
                StatusCode(200, "OK");
                return _topicsService.GetAllTopics(null, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in patch topic" });
            }
        }


        [HttpDelete("{topicId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTopic(int topicId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            
                if (!_topicsService.IsTopicExist(topicId))
                {
                    return StatusCode(404, new { message = "Topic not found" });
                }
                await _topicsService.DeleteTopic(topicId);
                return StatusCode(200, new { message = "OK" });
            
            
        }

        [Authorize]
        [HttpGet("{topicId}/childs")]
        public async Task<IActionResult> GetChilds(int topicId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            try
            {
                if (!_topicsService.IsTopicExist(topicId))
                {
                    return StatusCode(404, new { message = "Topic is not exist" });
                }
                var childs = _topicsService.GetTopicChilds(topicId);
                StatusCode(200, new { message = "OK" });
                return childs;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in getting topic's childs" });
            }
        }

        [Authorize]
        [HttpPost("{topicId}/childs")]
        public async Task<IActionResult> PostChilds([FromBody]int[] childs, int topicId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(401, new { message = "Post topics' childs model is incorrect" });
            }
            try
            {
                if (!_topicsService.IsTopicExist(topicId))
                {
                    return StatusCode(404, new { message = "Parent topic is not exist" });
                }
                await _topicsService.PostTopicChilds(topicId, childs);
                return _topicsService.GetOneTopic(topicId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in posting topic's childs" });
            }
        }

        
        [HttpDelete("{topicId}/childs")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteChilds([FromBody] int[] childs, int topicId)
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            if (_tokenService.IsTokenBlacklisted(_bearer_token))
            {
                return StatusCode(401, "");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(401, new { message = "Delete topics' childs model is incorrect" });
            }
            try
            {
                if (!_topicsService.IsTopicExist(topicId))
                {
                    return StatusCode(404, new { message = "Topic not found" });
                }
                await _topicsService.DeleteTopicChilds(childs);
                return _topicsService.GetOneTopic(topicId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong in deleting childs" });
            }
        }
    }
}
