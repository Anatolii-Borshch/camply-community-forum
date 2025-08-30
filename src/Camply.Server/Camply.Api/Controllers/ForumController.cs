using Camply.Api.Models.Helpers;
using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Domain.Enums;
using Camply.Shared.Dtos.Forum;
using Camply.Shared.Dtos.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    [ApiController]
    [Route("api/v1/forum")]
    [Authorize(Roles = RoleHelper.Poster + "," + RoleHelper.Admin)]
    public class ForumController : ControllerBase
    {
        private readonly IForumService _forumService;
        private readonly IAuthService _authService;
        
        public ForumController(IForumService forumService, IAuthService authService)
        {
            _forumService = forumService;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ForumPrevievDto>>>> GetForums(
            [FromQuery] string? title,
            [FromQuery] List<string>? tags,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20)
        {
            var request = new ForumSearchRequest(title, tags, skip, take);

            var forums = await _forumService.GetAllForums(request);

            return Ok(ApiResponse<IEnumerable<ForumPrevievDto>>.Ok(forums));
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<ForumDto>>> GetForum(Guid id)
        {
            var forum = await _forumService.GetForumById(id);
            return Ok(ApiResponse<ForumDto>.Ok(forum));
        }
        
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateForum([FromBody] CreateForumRequest request)
        {
            var userId = _authService.UserId;
            
            var forumRequest = new ForumCreateRequest(request.Title, request.Description, userId, request.Tags.Select(x => new TagDto(x)).ToList());
            
            await _forumService.CreateForum(forumRequest);

            return CreatedAtAction(nameof(GetForum), ApiResponse.Ok("Forum created successfully"));
        }


        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateForum(Guid id, [FromBody] UpdateForumRequest request)
        {
            var userId = _authService.UserId;

            var forumRequest = new ForumUpdateRequest(id, request.Title, request.Description, userId, request.Tags.Select(x => new TagDto(x)).ToList());

            await _forumService.UpdateForum(forumRequest);

            return Ok(ApiResponse.Ok("Forum updated successfully"));
        }


        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteForum(Guid id)
        {
            await _forumService.DeleteForum(id);

            return Ok(ApiResponse.Ok("Forum deleted successfully"));
        }
    }
}