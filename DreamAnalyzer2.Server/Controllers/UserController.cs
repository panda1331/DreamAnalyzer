using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces.Services;
using DreamAnalyzer2.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DreamAnalyzer2.Server.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileInfo()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _userService.GetUserInfo(userId);
            return Ok(ApiResponse<ProfileResponseDto>.SuccessResponse(response));
        }
    }
}
