using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Strategies;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Tests.Strategies
{
    public class CognitiveAnalysisStrategyTests
    {
        private readonly Mock<IDreamAiClient> _aiClientMock;
        private readonly CognitiveAnalysisStrategy _strategy;

        public CognitiveAnalysisStrategyTests()
        {
            _aiClientMock = new Mock<IDreamAiClient>();
            _strategy = new CognitiveAnalysisStrategy(_aiClientMock.Object);
        }

        [Fact]
        public async Task AnalyzeAsync_ShouldReturnValidAnalysisResponse()
        {
            // Arrange
            var dreamContent = "Тест сна для КПТ";
            var aiResponse = "{\"interpretation\": \"Ваши когнитивные искажения...\", \"mood\": \"Anxious\", \"symbols\": [\"экзамен\", \"опоздание\"]}";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(aiResponse);

            // Act
            var result = await _strategy.AnalyzeAsync(dreamContent);

            // Assert
            result.Should().NotBeNull();
            result.Interpretation.Should().Be("Ваши когнитивные искажения...");
            result.MoodName.Should().Be("Anxious");
        }
    }
}
