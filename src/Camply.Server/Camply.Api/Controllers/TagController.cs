using Camply.Api.Models.Helpers;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.Tag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    [ApiController]
    [Route("api/v1/tag")]
    [Authorize(Roles = RoleHelper.Admin)]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
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
            await _tagService.AddTagAsync(name);

            return Ok(ApiResponse.Ok("Tag created"));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> UpdateTag(Guid id, [FromBody] string name)
        {
            var tagRequest = new TagUpdateRequest(id, name);
            
            await _tagService.UpdateTagAsync(tagRequest);
            
            return Ok(ApiResponse.Ok("Tag updated"));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteTag(Guid id)
        {
            await _tagService.DeleteTagAsync(id);
            
            return Ok(ApiResponse.Ok("Tag deleted"));
        }
    }
}