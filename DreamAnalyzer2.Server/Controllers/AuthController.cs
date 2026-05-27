using DreamAnalyzer2.Application.DTOs.Requests.User;
using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces.Services;
using DreamAnalyzer2.Shared.Responses;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace DreamAnalyzer2.Server.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterDto registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDto loginDto)
        {
            //try
            //{
                var response = await _authService.LoginAsync(loginDto);
                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response));
            //}
            //catch (InvalidCredentialException ex)
            //{
            //    return Unauthorized(new { message = ex.Message });
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, new { message = "Internal server error" });
            //}
        }
    }
}
