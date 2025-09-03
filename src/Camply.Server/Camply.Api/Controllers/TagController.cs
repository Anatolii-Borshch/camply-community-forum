using Camply.Api.Models.Helpers;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Security.PrivacyServices;
using Camply.Shared.Dtos.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    [ApiController]
    [Route("api/v1/tag")]
    [Authorize(Roles = "Administrator")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IAuthService _authService;
        
        public TagController(ITagService tagService, AuthService authService)
        {
            _tagService = tagService;
            _authService = authService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetTags()
        {
            var tags = await _tagService.GetTagsAsync();
            
            return Ok(ApiResponse<IEnumerable<TagDto>>.Ok(tags));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateTag([FromBody] string name)
        {
            var userId = _authService.UserId;
            
            await _tagService.AddTagAsync(name, userId);

            return Ok(ApiResponse.Ok("Tag created"));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateTag(Guid id, [FromBody] string name)
        {
            var userId = _authService.UserId;
            
            var tagRequest = new TagUpdateRequest(id, name, userId);
            
            await _tagService.UpdateTagAsync(tagRequest);
            
            return Ok(ApiResponse.Ok("Tag updated"));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteTag(Guid id)
        {
            var userId = _authService.UserId;
            
            await _tagService.DeleteTagAsync(id, userId);
            
            return Ok(ApiResponse.Ok("Tag deleted"));
        }
    }
}