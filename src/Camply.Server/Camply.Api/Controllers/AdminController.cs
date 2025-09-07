using Camply.Api.Models.Helpers;
using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = RoleHelper.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AdminController(IUserService userService, IAuthService authService)
        {
            _authService = authService;
            _userService = userService;
        }
        
        [HttpPut("change-role/{userToChangeId:guid}")]
        public async Task<ActionResult<ApiResponse>> ChangeUserRole(Guid userToChangeId, [FromBody] ChangeUserRoleRequest request)
        {
            var userId = _authService.UserId;
            
            await _userService.ChangeUserRole(userToChangeId, userId, request.Role);
            
            return Ok(ApiResponse.Ok("Role changed successfully."));
        }
    }
}