using DreamAnalyzer2.Application.DTOs.Requests.Dreams;
using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces.Services;
using DreamAnalyzer2.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;

namespace DreamAnalyzer2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DreamsController : ControllerBase
    {
        private readonly IDreamService _service;

        public DreamsController(IDreamService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDream(CreateDreamDto createDreamDto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.CreateDreamAsync(userId, createDreamDto);
            return Ok(ApiResponse<DreamResponseDto>.SuccessResponse(response));
        }

        [HttpGet]
        public async Task<IActionResult> GetDreams()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.GetAllDreamsByUserIdAsync(userId);
            return Ok(ApiResponse<List<DreamResponseDto>>.SuccessResponse(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDream(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.GetDreamByIdAsync(userId, id);
            return Ok(ApiResponse<DreamResponseDto>.SuccessResponse(response));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDream(Guid id, UpdateDreamDto updateDreamDto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _service.UpdateDreamAsync(userId, id, updateDreamDto);
            return Ok(ApiResponse<DreamResponseDto>.SuccessResponse(response));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDream(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.DeleteDreamAsync(userId, id);
            return Ok(ApiResponse<string>.SuccessResponse("Dream deleted"));
        }
    }
}
