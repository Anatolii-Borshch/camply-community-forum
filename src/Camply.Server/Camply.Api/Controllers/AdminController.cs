using Camply.Api.Models.Helpers;
using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    /// <summary>
    /// Admin operations controller
    /// </summary>
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
        
        /// <summary>
        /// Change the role of a user.
        /// </summary>
        /// <param name="userToChangeId">The ID of the user whose role is being changed.</param>
        /// <param name="request">Contains the new role to assign.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <response code="200">Returns success with <see cref="ApiResponse.Success"/> = true</response>
        /// <response code="400">Returns validation failure with <see cref="ApiResponse.Errors"/></response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="403">Forbidden (user is not admin)</response>
        [HttpPut("change-role/{userToChangeId:guid}")]
        public async Task<ActionResult<ApiResponse>> ChangeUserRole(Guid userToChangeId, [FromBody] ChangeUserRoleRequest request)
        {
            var userId = _authService.UserId;
            
            await _userService.ChangeUserRole(userToChangeId, userId, request.Role);
            
            return Ok(ApiResponse.Ok("Role changed successfully."));
        }
    }
}