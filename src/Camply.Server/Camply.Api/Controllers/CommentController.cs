using Camply.Api.Models.Helpers;
using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
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

        [HttpGet("{postId:guid}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CommentDto>>>> GetPostCommentById(Guid postId, [FromQuery] int numberOfComments = 10)
        {
            var comments = await _commentService.GetPostCommentsAsync(postId, numberOfComments);
            
            return Ok(ApiResponse<IEnumerable<CommentDto>>.Ok(comments));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateComment(CreateCommentRequest request)
        {
            var userId = _authService.UserId;

            var commentRequest = new CommentCreateRequest(userId, request.PostId, request.Content, request.ParentCommentId);
            await _commentService.CreateComment(commentRequest);

            return Ok(ApiResponse.Ok("Post comment created successfully"));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateComment(Guid id,[FromBody] string content)
        {
            var userId = _authService.UserId;
            
            var commentRequest = new CommentUpdateRequest(userId, id, content);
            await _commentService.UpdateComment(commentRequest);
            
            return Ok(ApiResponse.Ok("Comment updated successfully"));
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteComment(Guid id)
        {
            var userId = _authService.UserId;
            
            await _commentService.DeleteComment(id, userId);
            
            return Ok(ApiResponse.Ok("Comment deleted successfully"));
        }
    }
}