using Camply.Api.Models.Helpers;
using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    /// <summary>
    /// Controller for managing comments on posts.
    /// </summary>
    [ApiController]
    [Route("api/v1/comment")]
    [Authorize(Roles = RoleHelper.Poster + "," + RoleHelper.Admin)]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IAuthService _authService;

        public CommentController(ICommentService commentService, IAuthService authService)
        {
            _commentService = commentService;
            _authService = authService;
        }

        /// <summary>
        /// Retrieves comments for a specific post.
        /// </summary>
        /// <param name="postId">ID of the post.</param>
        /// <param name="numberOfComments">Number of comments to retrieve (optional, default 10).</param>
        /// <returns>An <see>
        ///         <cref>ApiResponse{IEnumerable{CommentDto}}</cref>
        ///     </see>
        ///     with the list of comments.</returns>
        /// <response code="200">Returns the list of comments</response>
        [HttpGet("{postId:guid}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CommentDto>>>> GetPostCommentById(Guid postId, [FromQuery] int numberOfComments = 10)
        {
            var comments = await _commentService.GetPostCommentsAsync(postId, numberOfComments);
            
            return Ok(ApiResponse<IEnumerable<CommentDto>>.Ok(comments));
        }

        /// <summary>
        /// Creates a new comment on a post.
        /// </summary>
        /// <param name="request">Comment creation data.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/comment
        ///     {
        ///        "content": "This is a comment",
        ///        "postId": "c2b6a0d8-1234-4bfc-8aef-123456789abc",
        ///        "parentCommentId": null
        ///     }
        /// </remarks>
        /// <response code="200">Comment created successfully</response>
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateComment(CreateCommentRequest request)
        {
            var userId = _authService.UserId;

            var commentRequest = new CommentCreateRequest(userId, request.PostId, request.Content, request.ParentCommentId);
            await _commentService.CreateComment(commentRequest);

            return Ok(ApiResponse.Ok("Post comment created successfully"));
        }

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="id">ID of the comment to update.</param>
        /// <param name="content">Updated content of the comment.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <response code="200">Comment updated successfully</response>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateComment(Guid id,[FromBody] string content)
        {
            var userId = _authService.UserId;
            
            var commentRequest = new CommentUpdateRequest(userId, id, content);
            await _commentService.UpdateComment(commentRequest);
            
            return Ok(ApiResponse.Ok("Comment updated successfully"));
        }
        
        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="id">ID of the comment to delete.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <response code="200">Comment deleted successfully</response>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteComment(Guid id)
        {
            var userId = _authService.UserId;
            
            await _commentService.DeleteComment(id, userId);
            
            return Ok(ApiResponse.Ok("Comment deleted successfully"));
        }
    }
}