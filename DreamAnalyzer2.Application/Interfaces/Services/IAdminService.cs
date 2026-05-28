using DreamAnalyzer2.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<List<ProfileResponseDto>> GetAllUsersAsync();
        Task<List<DreamResponseDto>> GetAllDreamsAsync();
        Task DeleteUserAsync(Guid id);
        Task DeleteDreamAsync(Guid id);
    }
}
