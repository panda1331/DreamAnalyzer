using DreamAnalyzer2.Application.DTOs.Requests.User;
using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Interfaces.Security;
using DreamAnalyzer2.Application.Interfaces.Services;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Enums;
using DreamAnalyzer2.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.AccessControl;
using System.Security.Authentication;
using System.Text;

namespace DreamAnalyzer2.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;

        public AuthService(IUnitOfWork unitOfWork, IUserRepository userRepository, ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
                throw new InvalidCredentialException("Invalid email or password");
            var isCorrectPassword = _passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash);
            var token = _tokenGenerator.GenerateToken(user.Id, user.Email);

            return new AuthResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Token = token,
                Username = user.Username,
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new InvalidCredentialException("User with this email already exists.");
            var passwordHash = _passwordHasher.HashPassword(registerDto.Password);
            var user = new User(registerDto.Username, registerDto.Email, RoleType.User);
            user.SetPasswordHash(passwordHash);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var token = _tokenGenerator.GenerateToken(user.Id, user.Email);
            return new AuthResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Token = token,
                Username = user.Username,
            };
        }
    }
}
