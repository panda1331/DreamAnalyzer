using DreamAnalyzer2.Application.DTOs.Requests.User;
using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Interfaces.Security;
using DreamAnalyzer2.Application.Services;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Enums;
using DreamAnalyzer2.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace DreamAnalyzer2.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _tokenGeneratorMock = new Mock<ITokenGenerator>();

            _authService = new AuthService(
                _unitOfWorkMock.Object,
                _userRepositoryMock.Object,
                _tokenGeneratorMock.Object,
                _passwordHasherMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsAuthResponse()
        {
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test123"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(registerDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            _passwordHasherMock
                .Setup(h => h.HashPassword(registerDto.Password))
                .Returns("hashedPassword");

            _tokenGeneratorMock
                .Setup(g => g.GenerateToken(It.IsAny<Guid>(), registerDto.Email, It.IsAny<string>()))
                .Returns("fakeJwtToken");

            var response = await _authService.RegisterAsync(registerDto);

            response.Should().NotBeNull();
            response.Token.Should().NotBeNull();
            response.Email.Should().Be(registerDto.Email);
            response.Username.Should().Be(registerDto.Username);

            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidCredentialException()
        {
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "existing@example.com",
                Password = "Test123"
            };

            var existingUser = new User("existing", "existing@example.com", RoleType.User);

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(registerDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            Func<Task> act = async () => await _authService.RegisterAsync(registerDto);

            await act.Should().ThrowAsync<InvalidCredentialException>()
                .WithMessage("User with this email already exists.");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
        {
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Test123"
            };

            var user = new User("testuser", "test@example.com", RoleType.User);
            user.SetPasswordHash("hashedPassword");

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword(loginDto.Password, user.PasswordHash))
                .Returns(true);

            _tokenGeneratorMock
                .Setup(g => g.GenerateToken(user.Id, user.Email, user.Role.ToString()))
                .Returns("fakeJwtToken");

            var response = await _authService.LoginAsync(loginDto);

            response.Should().NotBeNull();
            response.Email.Should().Be(loginDto.Email);
            response.Token.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ThrowsInvalidCredentialException()
        {
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User("testuser", "test@example.com", RoleType.User);
            user.SetPasswordHash("hashedPassword");

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyPassword(loginDto.Password, user.PasswordHash))
                .Returns(false);

            Func<Task> act = async () => await _authService.LoginAsync(loginDto);

            await act.Should().ThrowAsync<InvalidCredentialException>()
                .WithMessage("Wrong password. Please try again.");
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentEmail_ThrowsInvalidCredentialException()
        {
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Test123"
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            Func<Task> act = async () => await _authService.LoginAsync(loginDto);

            await act.Should().ThrowAsync<InvalidCredentialException>()
                .WithMessage("No account found with this email. Please register.");
        }
    }
}
