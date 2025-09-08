using Camply.Api.Models.Helpers;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    /// <summary>
    /// Controller for managing tags.
    /// </summary>
    [ApiController]
    [Route("api/v1/tag")]
    [Authorize(Roles = RoleHelper.Admin)]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IAuthService _authService;
        
        public TagController(ITagService tagService, IAuthService authService)
        {
            _tagService = tagService;
            _authService = authService;
        }

        /// <summary>
        /// Retrieves all available tags.
        /// </summary>
        /// <returns>
        /// An <see cref="ApiResponse{IEnumerable{TagDto}}"/> containing the list of tags.
        /// </returns>
        /// <response code="200">Returns the list of tags</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetTags()
        {
            var tags = await _tagService.GetTagsAsync();
            
            return Ok(ApiResponse<IEnumerable<TagDto>>.Ok(tags));
        }

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="name">Name of the new tag.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/tag
        ///     "CSharp"
        /// 
        /// </remarks>
        /// <response code="200">Tag created successfully</response>
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateTag([FromBody] string name)
        {
            var userId = _authService.UserId;
            
            await _tagService.AddTagAsync(name, userId);

            return Ok(ApiResponse.Ok("Tag created"));
        }

        /// <summary>
        /// Updates an existing tag.
        /// </summary>
        /// <param name="id">ID of the tag to update.</param>
        /// <param name="name">Updated name of the tag.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/v1/tag/{id}
        ///     "DotNet"
        /// 
        /// </remarks>
        /// <response code="200">Tag updated successfully</response>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateTag(Guid id, [FromBody] string name)
        {
            var userId = _authService.UserId;
            
            var tagRequest = new TagUpdateRequest(id, name, userId);
            
            await _tagService.UpdateTagAsync(tagRequest);
            
            return Ok(ApiResponse.Ok("Tag updated"));
        }

        /// <summary>
        /// Deletes a tag by ID.
        /// </summary>
        /// <param name="id">ID of the tag to delete.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <response code="200">Tag deleted successfully</response>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteTag(Guid id)
        {
            var userId = _authService.UserId;
            
            await _tagService.DeleteTagAsync(id, userId);
            
            return Ok(ApiResponse.Ok("Tag deleted"));
        }
    }
}