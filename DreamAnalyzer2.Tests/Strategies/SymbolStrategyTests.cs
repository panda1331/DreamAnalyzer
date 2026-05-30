using DreamAnalyzer2.Application.Strategies;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Tests.Strategies
{
    public class SymbolStrategyTests
    {
        private readonly Mock<ISymbolRepository> _symbolRepositoryMock;
        private readonly SymbolStrategy _strategy;

        public SymbolStrategyTests()
        {
            _symbolRepositoryMock = new Mock<ISymbolRepository>();
            _strategy = new SymbolStrategy(_symbolRepositoryMock.Object);
        }

        [Fact]
        public async Task AnalyzeAsync_WhenSymbolFound_ReturnsAnalysisResponse()
        {
            // Arrange
            var dreamContent = "Мне приснилась кошка";
            var symbols = new List<DreamSymbol>
            {
                new DreamSymbol("кошка", "Независимость, таинственность", "Peaceful")
            };

            _symbolRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(symbols);

            // Act
            var result = await _strategy.AnalyzeAsync(dreamContent);

            // Assert
            result.Should().NotBeNull();
            result.Symbols.Should().Contain("кошка");
            result.MoodName.Should().Be("Peaceful");
        }

        [Fact]
        public async Task AnalyzeAsync_WhenNoSymbolFound_ReturnsEmptySymbols()
        {
            // Arrange
            var dreamContent = "Какой-то странный сон без символов";
            var symbols = new List<DreamSymbol>();

            _symbolRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(symbols);

            // Act
            var result = await _strategy.AnalyzeAsync(dreamContent);

            // Assert
            result.Should().NotBeNull();
            result.Symbols.Should().BeEmpty();
            result.Interpretation.Should().Be("No specific symbols found in your dream.");
        }
    }
}
