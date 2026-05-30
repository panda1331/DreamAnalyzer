using DreamAnalyzer2.Application.DTOs.Responses;
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
    public class AnalysisServiceTests
    {
        private readonly Mock<ISymbolRepository> _symbolRepositoryMock;
        private readonly Mock<IDreamRepository> _dreamRepositoryMock;
        private readonly Mock<IAnalysisRepository> _analysisRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAnalysisStrategy> _strategyMock;
        private readonly AnalysisService _analysisService;

        public AnalysisServiceTests()
        {
            _symbolRepositoryMock = new Mock<ISymbolRepository>();
            _dreamRepositoryMock = new Mock<IDreamRepository>();
            _analysisRepositoryMock = new Mock<IAnalysisRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _strategyMock = new Mock<IAnalysisStrategy>();

            _analysisService = new AnalysisService(
                _symbolRepositoryMock.Object,
                _dreamRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _analysisRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AnalyseDreamAsync_WithValidDream_ReturnsAnalysisResponse()
        {
            var userId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(userId, "Test Dream", "Test content", DateTime.UtcNow);

            var strategyResult = new AnalysisResponseDto
            {
                Title = "",
                Interpretation = "Test interpretation",
                MoodName = "Peaceful",
                MoodColor = "#378c3a",
                MoodDescription = "Peaceful description",
                Symbols = new List<string> { "кошка", "огонь" },
                Strategy = "symbols"
            };

            var analysis = new DreamAnalysis(dreamId, strategyResult.Interpretation, Mood.Peaceful);

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            _strategyMock
                .Setup(s => s.AnalyzeAsync(dream.Content))
                .ReturnsAsync(strategyResult);

            _analysisRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DreamAnalysis>()))
                .Returns(Task.CompletedTask);

            var response = await _analysisService.AnalyseDreamAsync(userId, dreamId, _strategyMock.Object);

            response.Should().NotBeNull();
            response.Interpretation.Should().Be("Test interpretation");
            response.MoodName.Should().Be("Peaceful");
            response.Symbols.Should().Contain("кошка");
            response.Strategy.Should().Be("symbols");

            _analysisRepositoryMock.Verify(r => r.AddAsync(It.IsAny<DreamAnalysis>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task AnalyseDreamAsync_WithNonExistentDream_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dream?)null);

            Func<Task> act = async () => await _analysisService.AnalyseDreamAsync(userId, dreamId, _strategyMock.Object);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("No such dream");
        }

        [Fact]
        public async Task AnalyseDreamAsync_WithAnotherUsersDream_ThrowsValidationException()
        {
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(anotherUserId, "Dream", "Content", DateTime.UtcNow);

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            Func<Task> act = async () => await _analysisService.AnalyseDreamAsync(userId, dreamId, _strategyMock.Object);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("You can analyze only your dreams");
        }

        [Fact]
        public async Task AnalyseDreamAsync_WhenStrategyReturnsSymbols_AddsSymbolsToAnalysis()
        {
            var userId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(userId, "Test Dream", "Мне приснилась кошка и огонь", DateTime.UtcNow);

            var strategyResult = new AnalysisResponseDto
            {
                Interpretation = "Interpretation",
                MoodName = "Peaceful",
                Symbols = new List<string> { "кошка", "огонь" },
                Strategy = "symbols"
            };

            var symbol1 = new DreamSymbol("кошка", "Независимость", "Peaceful");
            var symbol2 = new DreamSymbol("огонь", "Страсть", "Anxious");

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            _strategyMock
                .Setup(s => s.AnalyzeAsync(dream.Content))
                .ReturnsAsync(strategyResult);

            _symbolRepositoryMock
                .Setup(r => r.GetByNameAsync("кошка"))
                .ReturnsAsync(symbol1);

            _symbolRepositoryMock
                .Setup(r => r.GetByNameAsync("огонь"))
                .ReturnsAsync(symbol2);

            var response = await _analysisService.AnalyseDreamAsync(userId, dreamId, _strategyMock.Object);

            response.Should().NotBeNull();
            response.Symbols.Should().Contain("кошка");
            response.Symbols.Should().Contain("огонь");

            _symbolRepositoryMock.Verify(r => r.GetByNameAsync("кошка"), Times.Once);
            _symbolRepositoryMock.Verify(r => r.GetByNameAsync("огонь"), Times.Once);
        }

        [Fact]
        public async Task AnalyseDreamAsync_WhenDreamAlreadyHasAnalysis_UpdatesExistingAnalysis()
        {
            var userId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var existingAnalysis = new DreamAnalysis(dreamId, "Old interpretation", Mood.Anxious);
            var dream = new Dream(userId, "Test Dream", "Content", DateTime.UtcNow);
            dream.SetAnalysis(existingAnalysis);

            var strategyResult = new AnalysisResponseDto
            {
                Interpretation = "New interpretation",
                MoodName = "Peaceful",
                Symbols = new List<string> { "кошка" },
                Strategy = "symbols"
            };

            var symbol = new DreamSymbol("кошка", "Независимость", "Peaceful");

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            _strategyMock
                .Setup(s => s.AnalyzeAsync(dream.Content))
                .ReturnsAsync(strategyResult);

            _symbolRepositoryMock
                .Setup(r => r.GetByNameAsync("кошка"))
                .ReturnsAsync(symbol);

            var response = await _analysisService.AnalyseDreamAsync(userId, dreamId, _strategyMock.Object);

            response.Should().NotBeNull();
            response.Interpretation.Should().Be("New interpretation");
            response.MoodName.Should().Be("Peaceful");

            _analysisRepositoryMock.Verify(r => r.AddAsync(It.IsAny<DreamAnalysis>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task AnalyseDreamAsync_WhenSymbolNotFound_DoesNotAddToAnalysis()
        {
            var userId = Guid.NewGuid();
            var dreamId = Guid.NewGuid();
            var dream = new Dream(userId, "Test Dream", "Мне приснилась несуществующий символ", DateTime.UtcNow);

            var strategyResult = new AnalysisResponseDto
            {
                Interpretation = "Interpretation",
                MoodName = "Peaceful",
                Symbols = new List<string> { "несуществующий_символ" },
                Strategy = "symbols"
            };

            _dreamRepositoryMock
                .Setup(r => r.GetByIdAsync(dreamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dream);

            _strategyMock
                .Setup(s => s.AnalyzeAsync(dream.Content))
                .ReturnsAsync(strategyResult);

            _symbolRepositoryMock
                .Setup(r => r.GetByNameAsync("несуществующий_символ"))
                .ReturnsAsync((DreamSymbol?)null);

            var response = await _analysisService.AnalyseDreamAsync(userId, dreamId, _strategyMock.Object);

            response.Should().NotBeNull();

            _symbolRepositoryMock.Verify(r => r.GetByNameAsync("несуществующий_символ"), Times.Once);
        }
    }
}
