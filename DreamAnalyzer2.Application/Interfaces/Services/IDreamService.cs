using DreamAnalyzer2.Application.DTOs.Requests.Dreams;
using DreamAnalyzer2.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DreamAnalyzer2.Application.Interfaces.Services
{
    public interface IDreamService 
    {
        Task<DreamResponseDto> CreateDreamAsync(Guid userId, CreateDreamDto createDreamDto);
        Task<DreamResponseDto> UpdateDreamAsync(Guid userId, Guid id, UpdateDreamDto updateDreamDto);
        Task<List<DreamResponseDto>> GetAllDreamsByUserIdAsync(Guid userId);
        Task<DreamResponseDto> GetDreamByIdAsync(Guid userId, Guid id);
        Task DeleteDreamAsync(Guid userId, Guid id);
    }
}
