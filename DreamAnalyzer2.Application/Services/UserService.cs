using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Interfaces.Services;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Shared.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDreamRepository _dreamsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IDreamRepository dreamsRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _dreamsRepository = dreamsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");
            await _userRepository.DeleteAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var responses = new List<UserResponseDto>();
            foreach (var user in users) 
                responses.Add(ToResponse(user));
            return responses;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");
            return ToResponse(user);
        }

        public async Task<ProfileResponseDto> GetUserInfo(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");
            var createdDate = user.CreatedAt.ToString("dd.MM.yyyy");

            return new ProfileResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                RegistryDate = createdDate,
            };
        }

        public async Task<StatisticsResponseDto> GetUserStatisticsAsync(Guid userId)
        {
            var dreams = await _dreamsRepository.GetByUserIdAsync(userId);
            var analyzedDreams = dreams.Where(d => d.Analysis != null).ToList();

            var totalDreams = dreams.Count;
            var averageLength = Math.Round(dreams.Any() ? dreams.Average(d => d.Content.Length) : 0);
            
            var weekdayStats = new Dictionary<string, int>();
            foreach (var dream in dreams)
            {
                var weekday = dream.DreamDate.ToString("dddd", new System.Globalization.CultureInfo("ru-RU"));
                weekdayStats[weekday] = weekdayStats.GetValueOrDefault(weekday) + 1;
            }

            var moodStats = new Dictionary<string, int>();
            foreach(var dream in analyzedDreams)
            {
                var mood = dream.Analysis.Mood.Name;
                moodStats[mood] = moodStats.GetValueOrDefault(mood) + 1;
            } 

            var symbolStats = new Dictionary<string, int>();
            foreach ( var dream in analyzedDreams)
            {
                foreach( var symbol in dream.Analysis.Symbols)
                {
                    symbolStats[symbol.Name] = symbolStats.GetValueOrDefault(symbol.Name) + 1;
                }
            }

            var topSymbols = symbolStats
                .OrderByDescending(s => s.Value)
                .Take(5)
                .Select(s => new PopularSymbolDto { SymbolName = s.Key, Count = s.Value })
                .ToList();

            return new StatisticsResponseDto
            {
                TotalDreams = totalDreams,
                AverageDreamLength = averageLength,
                FirstDreamDate = dreams.MinBy(d => d.DreamDate)?.DreamDate.ToString("yyyy-MM-dd"),
                LastDreamDate = dreams.MaxBy(d => d.DreamDate)?.DreamDate.ToString("yyyy-MM-dd"),
                DreamsByWeekDay = weekdayStats,
                MoodDistribution = moodStats,
                PopularSymbols = topSymbols,
            };
        }

        private UserResponseDto ToResponse(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt.ToString("yyyy-MM-dd"),
                Role = user.Role.ToString(),
            };
        }
    }
}
