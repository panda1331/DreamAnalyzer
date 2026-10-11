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
            var dreamContent = "Мне приснилась кошка";
            var symbols = new List<DreamSymbol>
            {
                new DreamSymbol("кошка", "Независимость, таинственность", "Peaceful")
            };

            _symbolRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(symbols);

            var result = await _strategy.AnalyzeAsync(dreamContent);

            result.Should().NotBeNull();
            result.Symbols.Should().Contain("кошка");
            result.MoodName.Should().Be("Peaceful");
        }

        [Fact]
        public async Task AnalyzeAsync_WhenNoSymbolFound_ReturnsEmptySymbols()
        {
            var dreamContent = "Какой-то странный сон без символов";
            var symbols = new List<DreamSymbol>();

            _symbolRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(symbols);

            var result = await _strategy.AnalyzeAsync(dreamContent);

            result.Should().NotBeNull();
            result.Symbols.Should().BeEmpty();
            result.Interpretation.Should().Be("No specific symbols found in your dream.");
        }

        [Theory]
        [InlineData("Мне снилась кошка", new[] { "кошка" })]
        [InlineData("Я видел огонь и воду", new[] { "огонь", "вода" })]
        [InlineData("Полёт над морем", new[] { "полёт", "море" })]
        [InlineData("Пустой сон без символов", new string[0])]
        public async Task AnalyzeAsync_WithDifferentTexts_FindsExpectedSymbols(string dreamText, string[] expectedSymbols)
        {
            var symbols = new List<DreamSymbol>
            {
                new DreamSymbol("кошка", "Независимость", "Peaceful"),
                new DreamSymbol("огонь", "Страсть", "Anxious"),
                new DreamSymbol("вода", "Эмоции", "Peaceful"),
                new DreamSymbol("полёт", "Свобода", "Lucid"),
                new DreamSymbol("море", "Бесконечность", "Peaceful")
            };

            _symbolRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(symbols);

            var result = await _strategy.AnalyzeAsync(dreamText);

            result.Symbols.Should().BeEquivalentTo(expectedSymbols);
        }
    }
}
