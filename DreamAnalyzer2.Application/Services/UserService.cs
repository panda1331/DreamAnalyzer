using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces.Services;
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

        public UserService(IUserRepository userRepository, IDreamRepository dreamsRepository)
        {
            _userRepository = userRepository;
            _dreamsRepository = dreamsRepository;
        }

        public async Task<ProfileResponseDto> GetUserInfo(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var dreams = await _dreamsRepository.GetByUserIdAsync(userId);
            var dreamsCount = dreams.Count();

            return new ProfileResponseDto
            {
                Username = user.Username,
                Email = user.Email,
                RegistryDate = user.CreatedAt,
                DreamsCount = dreamsCount,
            };
        }
    }
}
