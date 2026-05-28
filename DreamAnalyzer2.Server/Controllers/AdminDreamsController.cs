using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces.Services;
using DreamAnalyzer2.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DreamAnalyzer2.Server.Controllers
{
    [ApiController]
    [Route("api/admin/dreams")]
    [Authorize(Roles = "Admin")]
    public class AdminDreamsController : ControllerBase
    {
        private readonly IDreamService _dreamService;

        public AdminDreamsController(IDreamService dreamService)
        {
            _dreamService = dreamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDreams()
        {
            var response = await _dreamService.GetAllDreamsAsync();
            return Ok(ApiResponse<List<DreamResponseDto>>.SuccessResponse(response));
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetDreamsByUser(Guid userId)
        {
            var response = await _dreamService.GetAllDreamsByUserIdAsync(userId);
            return Ok(ApiResponse<List<DreamResponseDto>>.SuccessResponse(response));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDreamById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _dreamService.GetDreamByIdAsync(userId, id);
            return Ok(ApiResponse<DreamResponseDto>.SuccessResponse(response));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDream(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _dreamService.DeleteDreamAsync(userId, id, isAdmin: true);
            return Ok(ApiResponse<string>.SuccessResponse("Dream deleted"));
        }
    }
}
