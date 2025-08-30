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

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginData>>> Login([FromBody] LoginRequest request)
        {
            var loginDto = new UserLoginDto(request.Password, request.Username, request.Email);
            
            var loginData = await _authService.Login(loginDto);
            
            return Ok(ApiResponse<LoginData>.Ok(loginData));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequest request)
        {
            var registerDto = new UserRegisterDto(request.Name, request.Surname, request.Username, request.Email, request.Password, request.BirthDate);
            
            await _authService.Register(registerDto);

            return Ok(ApiResponse.Ok("User registered successfully"));
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            await _authService.Logout();
            
            return Ok(ApiResponse.Ok("Endpoint mocked"));
        }
    }
}