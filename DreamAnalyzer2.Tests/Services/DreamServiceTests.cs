using DreamAnalyzer2.Application.DTOs.Requests.Dreams;
using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Services;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Shared.Shared;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Tests.Services
{
    public class DreamServiceTests
    {
        private readonly Mock<IDreamRepository> _dreamRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DreamService _dreamService;

        public DreamServiceTests()
        {
            _dreamRepositoryMock = new Mock<IDreamRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dreamService = new DreamService(_unitOfWorkMock.Object, _dreamRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateDreamAsync_WithValidData_ReturnsDreamResponse()
        {
            var userId = Guid.NewGuid();
            var createDto = new CreateDreamDto
            {
                Title = "Test Dream",
                Content = "Test content",
                DreamDate = DateTime.UtcNow
            };

            var response = await _dreamService.CreateDreamAsync(userId, createDto);

            response.Should().NotBeNull();
            response.Title.Should().Be(createDto.Title);
            response.Content.Should().Be(createDto.Content);

            _dreamRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Dream>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDreamAsync_UserDeletingOwnDream_DeletesDream()
        {
            var userId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(userId, "Title", "Content", DateTime.UtcNow);

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            await _dreamService.DeleteDreamAsync(userId, dreamId, isAdmin: false);

            _dreamRepositoryMock.Verify(r => r.DeleteAsync(dreamId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDreamAsync_UserDeletingAnothersDream_ThrowsValidationException()
        {
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(ownerId, "Title", "Content", DateTime.UtcNow);

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            Func<Task> act = async () => await _dreamService.DeleteDreamAsync(anotherUserId, dreamId, isAdmin: false);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("You can delete only your dreams");
        }

        [Fact]
        public async Task DeleteDreamAsync_AdminDeletingAnyDream_DeletesDream()
        {
            var ownerId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(ownerId, "Title", "Content", DateTime.UtcNow);

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            await _dreamService.DeleteDreamAsync(adminId, dreamId, isAdmin: true);

            _dreamRepositoryMock.Verify(r => r.DeleteAsync(dreamId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDreamByIdAsync_UserViewingOwnDream_ReturnsDream()
        {
            var userId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(userId, "Title", "Content", DateTime.UtcNow);

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            var response = await _dreamService.GetDreamByIdAsync(userId, dreamId, isAdmin: false);

            response.Should().NotBeNull();
            response.Title.Should().Be("Title");
        }

        [Fact]
        public async Task GetDreamByIdAsync_UserViewingAnothersDream_ThrowsValidationException()
        {
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(ownerId, "Title", "Content", DateTime.UtcNow);

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            Func<Task> act = async () => await _dreamService.GetDreamByIdAsync(anotherUserId, dreamId, isAdmin: false);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("You can only view your own dreams");
        }

        [Fact]
        public async Task UpdateDreamAsync_UserUpdatingOwnDream_UpdatesAndReturnsDream()
        {
            var userId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var originalDream = new Dream(userId, "Old Title", "Old Content", DateTime.UtcNow);

            var updateDto = new UpdateDreamDto
            {
                Title = "New Title",
                Content = "New Content",
                DreamDate = DateTime.UtcNow.AddDays(1)
            };

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(originalDream);

            var response = await _dreamService.UpdateDreamAsync(userId, dreamId, updateDto);

            response.Should().NotBeNull();
            response.Title.Should().Be(updateDto.Title);
            response.Content.Should().Be(updateDto.Content);

            _dreamRepositoryMock.Verify(r => r.Update(It.IsAny<Dream>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateDreamAsync_UserUpdatingAnothersDream_ThrowsValidationException()
        {
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(ownerId, "Title", "Content", DateTime.UtcNow);

            var updateDto = new UpdateDreamDto
            {
                Title = "New Title",
                Content = "New Content",
                DreamDate = DateTime.UtcNow
            };

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            Func<Task> act = async () => await _dreamService.UpdateDreamAsync(anotherUserId, dreamId, updateDto);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("You can update only your dreams");

            _dreamRepositoryMock.Verify(r => r.Update(It.IsAny<Dream>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDreamAsync_WithNonExistentDream_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var updateDto = new UpdateDreamDto
            {
                Title = "New Title",
                Content = "New Content",
                DreamDate = DateTime.UtcNow
            };

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dream?)null);

            Func<Task> act = async () => await _dreamService.UpdateDreamAsync(userId, dreamId, updateDto);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("No such dream");
        }
    }
}
