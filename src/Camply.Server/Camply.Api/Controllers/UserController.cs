using Camply.Api.Models.Helpers;
using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    [Authorize(Roles = RoleHelper.Poster + "," + RoleHelper.Admin)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetUserProfile(Guid id)
        {
            var profile = await _userService.GetUserByIdAsync(id);
            
            return Ok(ApiResponse<UserProfileDto>.Ok(profile));
        }
        
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
        {
            var userId = _authService.UserId;
            
            var profile = await _userService.GetUserByIdAsync(userId);
            
            return Ok(ApiResponse<UserProfileDto>.Ok(profile));
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = _authService.UserId;
            var updateRequest = new ProfileUpdateRequest(
                request.Name,
                request.Surname,
                request.Birthday,
                request.Username);

            await _userService.UpdateProfileAsync(updateRequest);
            
            return Ok(ApiResponse.Ok("Profile updated successfully"));
        }

        [HttpPut("password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = _authService.UserId;
            var resetRequest = new ResetPasswordRequest(userId, request.Password);

            await _userService.ChangePasswordAsync(resetRequest);
            
            return Ok(ApiResponse.Ok("Password changed successfully"));
        }

        [HttpDelete("account")]
        public async Task<ActionResult<ApiResponse>> DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            var userId = _authService.UserId;
            var deleteRequest = new AccountDeleteRequest(userId, request.Password);

            await _userService.DeleteAccount(deleteRequest);
            
            return Ok(ApiResponse.Ok("Account deleted successfully"));
        }
    }
}