using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Services;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Domain.ValueObjects;
using DreamAnalyzer2.Shared.Shared;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IDreamRepository> _dreamRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _dreamRepositoryMock = new Mock<IDreamRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userService = new UserService(
                _userRepositoryMock.Object,
                _dreamRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task GetUserInfo_WithValidUserId_ReturnsProfileResponse()
        {
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", Domain.Enums.RoleType.User);
            var createdDate = user.CreatedAt.ToString("dd.MM.yyyy");

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var response = await _userService.GetUserInfo(userId);

            response.Should().NotBeNull();
            response.Username.Should().Be("testuser");
            response.Email.Should().Be("test@example.com");
            response.RegistryDate.Should().Be(createdDate);
        }

        [Fact]
        public async Task GetUserInfo_WithNonExistentUserId_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            Func<Task> act = async () => await _userService.GetUserInfo(userId);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsListOfUsers()
        {
            var users = new List<User>
            {
                new User("user1", "user1@example.com", Domain.Enums.RoleType.User),
                new User("user2", "user2@example.com", Domain.Enums.RoleType.User),
                new User("admin", "admin@example.com", Domain.Enums.RoleType.Admin)
            };

            _userRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            var response = await _userService.GetAllUsersAsync();

            response.Should().NotBeNull();
            response.Should().HaveCount(3);
            response.Should().Contain(u => u.Username == "user1");
            response.Should().Contain(u => u.Username == "admin");
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ReturnsUserResponse()
        {
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", Domain.Enums.RoleType.User);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var response = await _userService.GetUserByIdAsync(userId);

            response.Should().NotBeNull();
            response?.Username.Should().Be("testuser");
            response?.Email.Should().Be("test@example.com");
            response?.Role.Should().Be("User");
        }

        [Fact]
        public async Task GetUserByIdAsync_WithNonExistentId_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            Func<Task> act = async () => await _userService.GetUserByIdAsync(userId);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task DeleteUserAsync_WithValidId_DeletesUser()
        {
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", Domain.Enums.RoleType.User);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            await _userService.DeleteUserAsync(userId);

            _userRepositoryMock.Verify(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WithNonExistentId_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            Func<Task> act = async () => await _userService.DeleteUserAsync(userId);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("User not found");
        }

        [Fact]
        public async Task GetUserStatisticsAsync_WithDreams_ReturnsCorrectStatistics()
        {
            var userId = Guid.NewGuid();
            var mood = Mood.Peaceful;

            var analysis = new DreamAnalysis(Guid.NewGuid(), "Test interpretation", mood);

            var symbols = new List<DreamSymbol>
            {
                new DreamSymbol("кошка", "Независимость", "Peaceful"),
                new DreamSymbol("огонь", "Страсть", "Anxious")
            };

            foreach (var symbol in symbols)
            {
                analysis.AddSymbol(symbol);
            }

            var dreams = new List<Dream>
            {
                new Dream(userId, "Сон 1", "Длинное содержание сна номер один", DateTime.UtcNow),
                new Dream(userId, "Сон 2", "Короткий сон", DateTime.UtcNow)
            };

            dreams[0].SetAnalysis(analysis);

            _dreamRepositoryMock
                .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dreams);

            var response = await _userService.GetUserStatisticsAsync(userId);

            response.Should().NotBeNull();
            response.TotalDreams.Should().Be(2);
            response.AverageDreamLength.Should().BeGreaterThan(0);
            response.DreamsByWeekDay.Should().NotBeNull();
            response.MoodDistribution.Should().NotBeNull();
        }

        [Fact]
        public async Task GetUserStatisticsAsync_WithNoDreams_ReturnsEmptyStatistics()
        {
            var userId = Guid.NewGuid();
            var emptyDreams = new List<Dream>();

            _dreamRepositoryMock
                .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyDreams);

            var response = await _userService.GetUserStatisticsAsync(userId);

            response.Should().NotBeNull();
            response.TotalDreams.Should().Be(0);
            response.AverageDreamLength.Should().Be(0);
            response.DreamsByWeekDay.Should().NotBeNull();
            response.MoodDistribution.Should().NotBeNull();
            response.PopularSymbols.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUserStatisticsAsync_WithAnalyzedDreams_ReturnsPopularSymbols()
        {
            var userId = Guid.NewGuid();
            var mood = Mood.Peaceful;

            var analysis = new DreamAnalysis(Guid.NewGuid(), "Interpretation", mood);

            var symbols = new List<DreamSymbol>
            {
                new DreamSymbol("кошка", "Независимость", "Peaceful"),
                new DreamSymbol("кошка", "Независимость", "Peaceful"),
                new DreamSymbol("вода", "Эмоции", "Peaceful")
            };

            foreach (var symbol in symbols)
            {
                analysis.AddSymbol(symbol);
            }

            var dreams = new List<Dream>
            {
                new Dream(userId, "Сон 1", "Content 1", DateTime.UtcNow)
            };

            dreams[0].SetAnalysis(analysis);

            _dreamRepositoryMock
                .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dreams);

            var response = await _userService.GetUserStatisticsAsync(userId);

            response.Should().NotBeNull();
            response.PopularSymbols.Should().NotBeEmpty();

            var topSymbol = response.PopularSymbols.First();
            topSymbol.SymbolName.Should().Be("кошка");
            topSymbol.Count.Should().Be(2);
        }
    }
}
