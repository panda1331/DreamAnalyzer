using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Strategies;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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
            var dreamContent = "Тест сна для КПТ";
            var aiResponse = "{\"interpretation\": \"Ваши когнитивные искажения...\", \"mood\": \"Anxious\", \"symbols\": [\"экзамен\", \"опоздание\"]}";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(aiResponse);

            var result = await _strategy.AnalyzeAsync(dreamContent);

            result.Should().NotBeNull();
            result.Interpretation.Should().Be("Ваши когнитивные искажения...");
            result.MoodName.Should().Be("Anxious");
        }

        [Fact]
        public async Task AnalyzeAsync_WhenAiReturnsInvalidJson_ThrowsException()
        {
            var dreamContent = "Тест сна";
            var invalidJson = "{ invalid json without quotes }";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidJson);

            Func<Task> act = async () => await _strategy.AnalyzeAsync(dreamContent);

            await act.Should().ThrowAsync<JsonException>();
        }


        [Fact]
        public async Task AnalyzeAsync_WhenAiReturnsEmptySymbols_ReturnsEmptyList()
        {
            var dreamContent = "Тест сна";
            var aiResponse = "{\"interpretation\": \"Толкование\", \"mood\": \"Peaceful\", \"symbols\": []}";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(aiResponse);

            var result = await _strategy.AnalyzeAsync(dreamContent);

            result.Should().NotBeNull();
            result.Symbols.Should().BeEmpty();
        }

        [Fact]
        public async Task AnalyzeAsync_WhenAiReturnsResponseWithoutBrackets_ThrowsException()
        {
            var dreamContent = "Тест сна";
            var responseWithoutBrackets = "interpretation: test, mood: Anxious";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseWithoutBrackets);

            Func<Task> act = async () => await _strategy.AnalyzeAsync(dreamContent);

            await act.Should().ThrowAsync<Exception>().WithMessage("*некорректный формат*");
        }

        [Fact]
        public async Task AnalyzeAsync_WhenAiReturnsNull_ThrowsException()
        {
            var dreamContent = "Тест сна";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null!);

            Func<Task> act = async () => await _strategy.AnalyzeAsync(dreamContent);

            await act.Should().ThrowAsync<Exception>();
        }
    }
}
