using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Strategies;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Tests.Strategies
{
    public class JungianAnalysisStrategyTests
    {
        private readonly Mock<IDreamAiClient> _aiClientMock;
        private readonly JungianAnalysisStrategy _strategy;

        public JungianAnalysisStrategyTests()
        {
            _aiClientMock = new Mock<IDreamAiClient>();
            _strategy = new JungianAnalysisStrategy(_aiClientMock.Object);
        }

        [Fact]
        public async Task AnalyzeAsync_ShouldReturnValidAnalysisResponse()
        {
            // Arrange
            var dreamContent = "Тест сна для Юнга";
            var aiResponse = "{\"interpretation\": \"Архетип Тени...\", \"mood\": \"Peaceful\", \"symbols\": [\"старик\", \"море\"]}";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(aiResponse);

            // Act
            var result = await _strategy.AnalyzeAsync(dreamContent);

            // Assert
            result.Should().NotBeNull();
            result.Interpretation.Should().Be("Архетип Тени...");
            result.MoodName.Should().Be("Peaceful");
            result.Symbols.Should().Contain("старик");
        }
    }
}
