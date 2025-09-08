using Camply.Api.Models.Helpers;
using Camply.Api.Models.Requests;
using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Shared.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Camply.Api.Controllers
{
    /// <summary>
    /// Controller for managing user accounts and profiles.
    /// </summary>
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
        
        /// <summary>
        /// Retrieves the profile of a specific user by ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>
        /// An <see cref="ApiResponse{UserProfileDto}"/> containing the user profile.
        /// </returns>
        /// <response code="200">Returns the user profile</response>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetUserProfile(Guid id)
        {
            var profile = await _userService.GetUserByIdAsync(id);
            
            return Ok(ApiResponse<UserProfileDto>.Ok(profile));
        }
        
        /// <summary>
        /// Retrieves the profile of the currently authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="ApiResponse{UserProfileDto}"/> containing the current user profile.
        /// </returns>
        /// <response code="200">Returns the authenticated user's profile</response>
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
        {
            var userId = _authService.UserId;
            
            var profile = await _userService.GetUserByIdAsync(userId);
            
            return Ok(ApiResponse<UserProfileDto>.Ok(profile));
        }

        /// <summary>
        /// Updates the profile of the currently authenticated user.
        /// </summary>
        /// <param name="request">The profile update request.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/v1/user
        ///     {
        ///         "name": "John",
        ///         "surname": "Doe",
        ///         "birthday": "1995-05-12",
        ///         "username": "john_doe"
        ///     }
        /// </remarks>
        /// <response code="200">Profile updated successfully</response>
        [HttpPut]
        public async Task<ActionResult<ApiResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = _authService.UserId;
            var updateRequest = new ProfileUpdateRequest(
                userId,
                request.Name,
                request.Surname,
                request.Birthday,
                request.Username);

            await _userService.UpdateProfileAsync(updateRequest);
            
            return Ok(ApiResponse.Ok("Profile updated successfully"));
        }

        /// <summary>
        /// Changes the password of the currently authenticated user.
        /// </summary>
        /// <param name="request">The change password request.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/v1/user/password
        ///     {
        ///         "password": "newStrongPassword123"
        ///     }
        /// </remarks>
        /// <response code="200">Password changed successfully</response>
        [HttpPut("password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = _authService.UserId;
            var resetRequest = new ResetPasswordRequest(userId, request.Password);

            await _userService.ChangePasswordAsync(resetRequest);
            
            return Ok(ApiResponse.Ok("Password changed successfully"));
        }

        /// <summary>
        /// Deletes the currently authenticated user's account.
        /// </summary>
        /// <param name="request">The delete account request (requires password confirmation).</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /api/v1/user
        ///     {
        ///         "password": "userPassword123"
        ///     }
        /// </remarks>
        /// <response code="200">Account deleted successfully</response>
        [HttpDelete]
        public async Task<ActionResult<ApiResponse>> DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            var userId = _authService.UserId;
            var deleteRequest = new AccountDeleteRequest(userId, request.Password);

            await _userService.DeleteAccount(deleteRequest);
            
            return Ok(ApiResponse.Ok("Account deleted successfully"));
        }
    }
}