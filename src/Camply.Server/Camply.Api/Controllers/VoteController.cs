using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.Vote;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    /// <summary>
    /// Controller for managing forum votes and vote options.
    /// </summary>
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
        
        /// <summary>
        /// Retrieves a list of votes with optional filters.
        /// </summary>
        /// <param name="forumId">Filter by forum ID (optional).</param>
        /// <param name="authorId">Filter by author ID (optional).</param>
        /// <param name="voteId">Filter by vote ID (optional).</param>
        /// <param name="title">Filter by vote title (optional).</param>
        /// <param name="skip">Number of records to skip for pagination (default = 0).</param>
        /// <param name="take">Number of records to return (default = 20).</param>
        /// <param name="orderBy">Property name to order results by (optional).</param>
        /// <param name="orderDescending">Whether to order results descending (default = false).</param>
        /// <returns>List of votes.</returns>
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

        /// <summary>
        /// Creates a new vote in a forum.
        /// </summary>
        /// <param name="request">Vote creation request containing title, forum ID, and options.</param>
        /// <returns>Success message.</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateVote([FromBody] CreateVoteRequest request)
        {
            var userId = _authService.UserId;
            var createRequest = new VoteCreateRequest(request.Title, request.ForumId, userId, request.VoteOptions);

            await _voteService.CreateVote(createRequest);
            
            return Ok(ApiResponse.Ok("Vote created successfully"));
        }

        /// <summary>
        /// Updates an existing vote.
        /// </summary>
        /// <param name="id">Vote ID.</param>
        /// <param name="request">Update request containing new title.</param>
        /// <returns>Success message.</returns>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateVote(Guid id, [FromBody] UpdateVoteRequest request)
        {
            var userId = _authService.UserId;
            var updateRequest = new VoteUpdateRequest(id, userId, request.Title);

            await _voteService.UpdateVote(updateRequest);
            
            return Ok(ApiResponse.Ok("Vote updated successfully"));
        }

        /// <summary>
        /// Deletes a vote by ID.
        /// </summary>
        /// <param name="id">Vote ID.</param>
        /// <returns>Success message.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteVote(Guid id)
        {
            var userId = _authService.UserId;
            
            await _voteService.DeleteVote(id, userId);
            
            return Ok(ApiResponse.Ok("Vote deleted successfully"));
        }

        /// <summary>
        /// Toggles a user's vote for a specific option.
        /// </summary>
        /// <param name="optionId">Vote option ID.</param>
        /// <returns>True if vote was added, false if removed.</returns>
        [HttpPatch("{optionId:guid}/toggle")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleVote(Guid optionId)
        {
            var userId = _authService.UserId;
            
            var result = await _voteService.Vote(optionId, userId);
            
            return Ok(ApiResponse<bool>.Ok(result, result ? "Vote added" : "Vote removed"));
        }

        /// <summary>
        /// Updates a vote option.
        /// </summary>
        /// <param name="id">Vote option ID.</param>
        /// <param name="request">Request containing updated name and index.</param>
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

        /// <summary>
        /// Deletes a vote option by ID.
        /// </summary>
        /// <param name="id">Vote option ID.</param>
        /// <returns>Success message.</returns>ч
        [HttpDelete("option/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteVoteOption(Guid id)
        {
            var userId = _authService.UserId;
            
            await _voteService.DeleteVoteOption(id, userId);
            
            return Ok(ApiResponse.Ok("Vote option deleted successfully"));
        }
    }
}