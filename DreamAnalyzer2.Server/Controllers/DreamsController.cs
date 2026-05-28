using DreamAnalyzer2.Application.DTOs.Requests.Dreams;
using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
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
        private readonly IDreamService _dreamService;
        private readonly IStrategyFactory _strategyFactory;
        private readonly IAnalysisService _analysisService;

        public DreamsController(IDreamService dreamService, IAnalysisService analysisService, IStrategyFactory strategyFactory)
        {
            _dreamService = dreamService;
            _analysisService = analysisService;
            _strategyFactory = strategyFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDream(CreateDreamDto createDreamDto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _dreamService.CreateDreamAsync(userId, createDreamDto);
            return Ok(ApiResponse<DreamResponseDto>.SuccessResponse(response));
        }

        [HttpGet]
        public async Task<IActionResult> GetDreams()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _dreamService.GetAllDreamsByUserIdAsync(userId);
            return Ok(ApiResponse<List<DreamResponseDto>>.SuccessResponse(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDream(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var isAdmin = userRole == "Admin";

            var response = await _dreamService.GetDreamByIdAsync(userId, id, isAdmin);
            return Ok(ApiResponse<DreamResponseDto>.SuccessResponse(response));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDream(Guid id, UpdateDreamDto updateDreamDto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _dreamService.UpdateDreamAsync(userId, id, updateDreamDto);
            return Ok(ApiResponse<DreamResponseDto>.SuccessResponse(response));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDream(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _dreamService.DeleteDreamAsync(userId, id, isAdmin: false);
            return Ok(ApiResponse<string>.SuccessResponse("Dream deleted"));
        }

        [HttpPost("{id}/analyze")]
        public async Task<IActionResult> AnalyzeDream(Guid id, [FromQuery] string strategyType, [FromQuery] string? book = null)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var strategy = _strategyFactory.GetStrategy(strategyType, book);
            var result = await _analysisService.AnalyseDreamAsync(userId, id, strategy);
            return Ok(ApiResponse<AnalysisResponseDto>.SuccessResponse(result));
        }
    }
}
