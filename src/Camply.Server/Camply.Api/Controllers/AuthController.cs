using Camply.Api.Models.Responses;
using Camply.Application.Contracts.Services;
using Camply.Application.Security;
using Camply.Shared.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Camply.Api.Models.Requests.LoginRequest;
using RegisterRequest = Camply.Api.Models.Requests.RegisterRequest;

namespace Camply.Api.Controllers
{
    /// <summary>
    /// Controller responsible for authentication operations.
    /// </summary>
    [ApiController]
    [Route("api/v1/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Logs in a user using username/email and password.
        /// </summary>
        /// <param name="request">Login credentials.</param>
        /// <returns>An <see cref="ApiResponse{LoginData}"/> containing JWT and user info.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/login
        ///     {
        ///        "username": "user1",
        ///        "email": "user1@example.com",
        ///        "password": "password123"
        ///     }
        /// </remarks>
        /// <response code="200">Login successful, returns <see cref="ApiResponse{LoginData}"/></response>
        /// <response code="400">Validation failed, returns <see cref="ApiResponse.Errors"/></response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginData>>> Login([FromBody] LoginRequest request)
        {
            var loginDto = new UserLoginDto(request.Password, request.Username, request.Email);
            
            var loginData = await _authService.Login(loginDto);
            
            return Ok(ApiResponse<LoginData>.Ok(loginData));
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">User registration details.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating success or failure.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/auth/register
        ///     {
        ///        "name": "John",
        ///        "surname": "Doe",
        ///        "username": "johndoe",
        ///        "email": "john@example.com",
        ///        "password": "password123",
        ///        "birthDate": "1990-01-01"
        ///     }
        /// </remarks>
        /// <response code="200">Registration successful</response>
        /// <response code="400">Validation failed, returns <see cref="ApiResponse.Errors"/></response>
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequest request)
        {
            var registerDto = new UserRegisterDto(request.Name, request.Surname, request.Username, request.Email, request.Password, request.BirthDate);
            
            await _authService.Register(registerDto);

            return Ok(ApiResponse.Ok("User registered successfully"));
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>An <see cref="ApiResponse"/> confirming logout.</returns>
        /// <response code="200">Logout successful</response>
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            await _authService.Logout();
            
            return Ok(ApiResponse.Ok("Endpoint mocked"));
        }
    }
}