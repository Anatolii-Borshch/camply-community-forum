using Camply.Api.Models.Helpers;
using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    [ApiController]
    [Route("api/v1/post")]
    [Authorize(Roles = RoleHelper.Poster + "," + RoleHelper.Admin)]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IAuthService _authService;
        
        public PostController(IPostService postService, IAuthService authService)
        {
            _postService = postService;
            _authService = authService;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostDto>>>> GetPosts(
            [FromQuery] Guid? forumId,
            [FromQuery] Guid? authorId,
            [FromQuery] string? title,
            [FromQuery] bool? isPinned,
            [FromQuery] DateTime? createdAfter,
            [FromQuery] DateTime? createdBefore,
            [FromQuery] string orderBy = "createdDate",
            [FromQuery] bool descending = true,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20)
        {
            var request = new ForumPostsSearchRequest(
                forumId, authorId, title, isPinned,
                createdAfter, createdBefore,
                orderBy, descending, skip, take);

            var posts = await _postService.GetForumPostsAsync(request);
            return Ok(ApiResponse<IEnumerable<PostDto>>.Ok(posts));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreatePost([FromBody] CreatePostRequest request)
        {
            var userId = _authService.UserId;
            var newRequest = new PostCreateRequest(request.Title, request.Description, userId);
            
            await _postService.CreatePost(newRequest);
            
            return Ok(ApiResponse.Ok("Post created successfully"));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdatePost(Guid id, [FromBody] UpdatePostRequest request)
        {
            var userId = _authService.UserId;
            var updateRequest = new PostUpdateRequest(id, request.Title, request.Description, userId);
            
            await _postService.UpdatePost(updateRequest);
            
            return Ok(ApiResponse.Ok("Post updated successfully"));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeletePost(Guid id)
        {
            var userId = _authService.UserId;
            
            await _postService.DeletePost(userId, id);
            
            return Ok(ApiResponse.Ok("Post deleted successfully"));
        }

        [HttpPatch("{id:guid}/pin")]
        public async Task<ActionResult<ApiResponse<bool>>> PinPost(Guid id)
        {
            var userId = _authService.UserId;
            
            var result = await _postService.PinPost(userId, id);
            
            return Ok(ApiResponse<bool>.Ok(result, result ? "Post pinned" : "Post unpinned"));
        }

        [HttpPatch("{id:guid}/like")]
        public async Task<ActionResult<ApiResponse<bool>>> LikePost(Guid id)
        {
            var userId = _authService.UserId;
            
            var result = await _postService.LikePost(userId, id);
            
            return Ok(ApiResponse<bool>.Ok(result, result ? "Post liked" : "Post unliked"));
        }

        [HttpPatch("{id:guid}/save")]
        public async Task<ActionResult<ApiResponse<bool>>> SavePost(Guid id)
        {
            var userId = _authService.UserId;
            
            var result = await _postService.SavePost(userId, id);
            
            return Ok(ApiResponse<bool>.Ok(result, result ? "Post saved" : "Post unsaved"));
        }
    }
}