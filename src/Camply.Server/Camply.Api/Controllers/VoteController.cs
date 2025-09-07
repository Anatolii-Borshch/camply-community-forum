using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.Vote;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    [ApiController]
    [Route("api/v1/vote")]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService _voteService;
        private readonly IAuthService _authService;
        
        public VoteController(IVoteService voteService, IAuthService authService)
        {
            _voteService = voteService;
            _authService = authService;
        }
        
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<VoteDto>>>> GetVotes(
            [FromQuery] Guid? forumId,
            [FromQuery] Guid? authorId,
            [FromQuery] Guid? voteId,
            [FromQuery] string? title,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            [FromQuery] string? orderBy = null,
            [FromQuery] bool orderDescending = false)
        {
            var userId = _authService.UserId;
            
            var request = new ForumVotesSearchRequest(userId ,forumId, authorId, voteId, title, skip, take, orderBy, orderDescending);

            var votes = await _voteService.GetForumVotesAsync(request);
            
            return Ok(ApiResponse<IEnumerable<VoteDto>>.Ok(votes));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateVote([FromBody] CreateVoteRequest request)
        {
            var userId = _authService.UserId;
            var createRequest = new VoteCreateRequest(request.Title, request.ForumId, userId, request.VoteOptions);

            await _voteService.CreateVote(createRequest);
            
            return Ok(ApiResponse.Ok("Vote created successfully"));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateVote(Guid id, [FromBody] UpdateVoteRequest request)
        {
            var userId = _authService.UserId;
            var updateRequest = new VoteUpdateRequest(id, userId, request.Title);

            await _voteService.UpdateVote(updateRequest);
            
            return Ok(ApiResponse.Ok("Vote updated successfully"));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteVote(Guid id)
        {
            var userId = _authService.UserId;
            
            await _voteService.DeleteVote(id, userId);
            
            return Ok(ApiResponse.Ok("Vote deleted successfully"));
        }

        [HttpPatch("{optionId:guid}/toggle")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleVote(Guid optionId)
        {
            var userId = _authService.UserId;
            
            var result = await _voteService.Vote(optionId, userId);
            
            return Ok(ApiResponse<bool>.Ok(result, result ? "Vote added" : "Vote removed"));
        }

        [HttpPatch("option/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateVoteOption(Guid id, [FromBody] UpdateVoteOptionRequest request)
        {
            var userId = _authService.UserId;
            var updateRequest = new VoteOptionUpdateRequest(
                id,
                userId,
                request.Name,
                request.Index);

            await _voteService.UpdateVoteOption(updateRequest);
            
            return Ok(ApiResponse.Ok("Vote option updated successfully"));
        }

        [HttpDelete("option/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteVoteOption(Guid id)
        {
            var userId = _authService.UserId;
            
            await _voteService.DeleteVoteOption(id, userId);
            
            return Ok(ApiResponse.Ok("Vote option deleted successfully"));
        }
    }
}