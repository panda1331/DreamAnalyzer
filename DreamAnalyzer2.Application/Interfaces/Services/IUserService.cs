using DreamAnalyzer2.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ProfileResponseDto> GetUserInfo(Guid userId); 
        Task<StatisticsResponseDto> GetUserStatisticsAsync(Guid userId);

        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(Guid userId);
        Task DeleteUserAsync(Guid userId);
    }
}
