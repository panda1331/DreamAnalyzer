using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Strategies;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Tests.Strategies
{
    public class FreudianAnalysisStrategyTests
    {
        private readonly Mock<IDreamAiClient> _aiClientMock;
        private readonly FreudianAnalysisStrategy _strategy;

        public FreudianAnalysisStrategyTests()
        {
            _aiClientMock = new Mock<IDreamAiClient>();
            _strategy = new FreudianAnalysisStrategy(_aiClientMock.Object);
        }

        [Fact]
        public async Task AnalyzeAsync_ShouldReturnValidAnalysisResponse()
        {
            var dreamContent = "Тест сна для Фрейда";
            var aiResponse = "{\"interpretation\": \"Этот сон символизирует...\", \"mood\": \"Anxious\", \"symbols\": [\"лестница\", \"змея\"]}";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(aiResponse);

            var result = await _strategy.AnalyzeAsync(dreamContent);

            result.Should().NotBeNull();
            result.Interpretation.Should().Be("Этот сон символизирует...");
            result.MoodName.Should().Be("Anxious");
            result.Symbols.Should().Contain("лестница");
        }

        [Fact]
        public async Task AnalyzeAsync_WhenAiReturnsInvalidJson_ThrowsException()
        {
            var dreamContent = "Тест сна";
            var invalidJson = "Not a valid JSON";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidJson);

            Func<Task> act = async () => await _strategy.AnalyzeAsync(dreamContent);

            await act.Should().ThrowAsync<Exception>();
        }
    }
}
