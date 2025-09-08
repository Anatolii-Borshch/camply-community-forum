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
    /// <summary>
    /// Controller for managing forums.
    /// </summary>
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

        /// <summary>
        /// Get a list of forums with optional filtering.
        /// </summary>
        /// <param name="title">Optional forum title filter.</param>
        /// <param name="tags">Optional list of tag IDs to filter by.</param>
        /// <param name="skip">Number of items to skip (pagination).</param>
        /// <param name="take">Number of items to take (pagination).</param>
        /// <returns>
        /// A list of forum previews wrapped in <see cref="ApiResponse{T}"/>.
        /// </returns>
        /// <response code="200">Returns a list of forums.</response>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ForumPrevievDto>>>> GetForums(
            [FromQuery] string? title,
            [FromQuery] List<Guid>? tags,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20)
        {
            var request = new ForumSearchRequest(title, tags, skip, take);

            var forums = await _forumService.GetAllForums(request);

            return Ok(ApiResponse<IEnumerable<ForumPrevievDto>>.Ok(forums));
        }

        /// <summary>
        /// Get a forum by its ID.
        /// </summary>
        /// <param name="id">Forum ID.</param>
        /// <returns>
        /// Forum details wrapped in <see cref="ApiResponse{T}"/>.
        /// </returns>
        /// <response code="200">Forum found and returned.</response>
        /// <response code="404">Forum not found.</response>
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<ForumDto>>> GetForum(Guid id)
        {
            var forum = await _forumService.GetForumById(id);
            return Ok(ApiResponse<ForumDto>.Ok(forum));
        }
        
        /// <summary>
        /// Creates a new forum.
        /// </summary>
        /// <param name="request">Forum creation data.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/forum
        ///     {
        ///        "title": "Tech Forum",
        ///        "description": "A forum about technology",
        ///        "tags": ["c2b6a0d8-1234-4bfc-8aef-123456789abc"]
        ///     }
        /// </remarks>
        /// <response code="200">Forum created successfully.</response>
        /// <response code="400">Validation error.</response>
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateForum([FromBody] CreateForumRequest request)
        {
            var userId = _authService.UserId;
            
            var forumRequest = new ForumCreateRequest(request.Title, request.Description, userId, request.Tags.Select(x => new TagDto(x, string.Empty)).ToList());
            
            await _forumService.CreateForum(forumRequest);

            return Ok(ApiResponse.Ok("Forum created successfully"));
        }
        
        /// <summary>
        /// Updates an existing forum.
        /// </summary>
        /// <param name="id">ID of the forum to update.</param>
        /// <param name="request">Forum update data.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/v1/forum/{id}
        ///     {
        ///        "title": "Updated Forum",
        ///        "description": "Updated description",
        ///        "tagsId": ["c2b6a0d8-1234-4bfc-8aef-123456789abc"]
        ///     }
        /// </remarks>
        /// <response code="200">Forum updated successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="404">Forum not found.</response>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateForum(Guid id, [FromBody] UpdateForumRequest request)
        {
            var userId = _authService.UserId;

            var forumRequest = new ForumUpdateRequest(id, request.Title, request.Description, userId, request.TagsId.Select(x => new TagDto(x, string.Empty)).ToList());

            await _forumService.UpdateForum(forumRequest);

            return Ok(ApiResponse.Ok("Forum updated successfully"));
        }

        /// <summary>
        /// Deletes a forum.
        /// </summary>
        /// <param name="id">ID of the forum to delete.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <response code="200">Forum deleted successfully.</response>
        /// <response code="404">Forum not found.</response>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteForum(Guid id)
        {
            var userId = _authService.UserId;
            
            await _forumService.DeleteForum(id, userId);

            return Ok(ApiResponse.Ok("Forum deleted successfully"));
        }
    }
}