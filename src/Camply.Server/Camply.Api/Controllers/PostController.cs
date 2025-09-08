using Camply.Api.Models.Helpers;
using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    /// <summary>
    /// Controller for managing posts in forums.
    /// </summary>
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
        
        /// <summary>
        /// Retrieves posts with optional filtering and pagination.
        /// </summary>
        /// <param name="forumId">Filter by forum ID.</param>
        /// <param name="authorId">Filter by author ID.</param>
        /// <param name="title">Filter by post title (partial match).</param>
        /// <param name="isPinned">Filter by pinned status.</param>
        /// <param name="createdAfter">Return posts created after this date.</param>
        /// <param name="createdBefore">Return posts created before this date.</param>
        /// <param name="orderBy">Field to order by (default: createdDate).</param>
        /// <param name="descending">Whether to sort in descending order (default: true).</param>
        /// <param name="skip">Number of records to skip (for pagination).</param>
        /// <param name="take">Number of records to take (default: 20).</param>
        /// <returns>
        /// An <see cref="ApiResponse{IEnumerable{PostDto}}"/> with the list of posts.
        /// </returns>
        /// <response code="200">Returns the filtered list of posts</response>
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

        /// <summary>
        /// Creates a new post in a forum.
        /// </summary>
        /// <param name="request">Post creation data.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/post
        ///     {
        ///        "title": "My first post",
        ///        "description": "This is the body of the post",
        ///        "forumId": "a1b2c3d4-5678-90ab-cdef-123456789abc"
        ///     }
        /// </remarks>
        /// <response code="200">Post created successfully</response>
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreatePost([FromBody] CreatePostRequest request)
        {
            var userId = _authService.UserId;
            var newRequest = new PostCreateRequest(request.Title, request.Description, userId, request.ForumId);
            
            await _postService.CreatePost(newRequest);
            
            return Ok(ApiResponse.Ok("Post created successfully"));
        }

        /// <summary>
        /// Updates an existing post.
        /// </summary>
        /// <param name="id">ID of the post to update.</param>
        /// <param name="request">Updated post data.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <response code="200">Post updated successfully</response>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdatePost(Guid id, [FromBody] UpdatePostRequest request)
        {
            var userId = _authService.UserId;
            var updateRequest = new PostUpdateRequest(id, request.Title, request.Description, userId);
            
            await _postService.UpdatePost(updateRequest);
            
            return Ok(ApiResponse.Ok("Post updated successfully"));
        }

        /// <summary>
        /// Deletes a post by ID.
        /// </summary>
        /// <param name="id">ID of the post to delete.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <response code="200">Post deleted successfully</response>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeletePost(Guid id)
        {
            var userId = _authService.UserId;
            
            await _postService.DeletePost(userId, id);
            
            return Ok(ApiResponse.Ok("Post deleted successfully"));
        }

        /// <summary>
        /// Pins or unpins a post.
        /// </summary>
        /// <param name="id">ID of the post to pin/unpin.</param>
        /// <returns>
        /// An <see>
        ///     <cref>ApiResponse{bool}</cref>
        /// </see>
        /// indicating whether the post was pinned (true) or unpinned (false).
        /// </returns>
        /// <response code="200">Post pin status updated successfully</response>
        [HttpPatch("{id:guid}/pin")]
        public async Task<ActionResult<ApiResponse<bool>>> PinPost(Guid id)
        {
            var userId = _authService.UserId;
            
            var result = await _postService.PinPost(userId, id);
            
            return Ok(ApiResponse<bool>.Ok(result, result ? "Post pinned" : "Post unpinned"));
        }

        /// <summary>
        /// Likes or unlikes a post.
        /// </summary>
        /// <param name="id">ID of the post to like/unlike.</param>
        /// <returns>
        /// An <see>
        ///     <cref>ApiResponse{bool}</cref>
        /// </see>
        /// indicating whether the post was liked (true) or unliked (false).
        /// </returns>
        /// <response code="200">Post like status updated successfully</response>
        [HttpPatch("{id:guid}/like")]
        public async Task<ActionResult<ApiResponse<bool>>> LikePost(Guid id)
        {
            var userId = _authService.UserId;
            
            var result = await _postService.LikePost(userId, id);
            
            return Ok(ApiResponse<bool>.Ok(result, result ? "Post liked" : "Post unliked"));
        }

        /// <summary>
        /// Saves or unsaves a post.
        /// </summary>
        /// <param name="id">ID of the post to save/unsave.</param>
        /// <returns>
        /// An <see>
        ///     <cref>ApiResponse{bool}</cref>
        /// </see>
        /// indicating whether the post was saved (true) or unsaved (false).
        /// </returns>
        /// <response code="200">Post save status updated successfully</response>
        [HttpPatch("{id:guid}/save")]
        public async Task<ActionResult<ApiResponse<bool>>> SavePost(Guid id)
        {
            var userId = _authService.UserId;
            
            var result = await _postService.SavePost(userId, id);
            
            return Ok(ApiResponse<bool>.Ok(result, result ? "Post saved" : "Post unsaved"));
        }
    }
}