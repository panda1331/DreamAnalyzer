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
            var dreamContent = "Тест сна для Юнга";
            var aiResponse = "{\"interpretation\": \"Архетип Тени...\", \"mood\": \"Peaceful\", \"symbols\": [\"старик\", \"море\"]}";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(aiResponse);

            var result = await _strategy.AnalyzeAsync(dreamContent);

            result.Should().NotBeNull();
            result.Interpretation.Should().Be("Архетип Тени...");
            result.MoodName.Should().Be("Peaceful");
            result.Symbols.Should().Contain("старик");
        }

        [Fact]
        public async Task AnalyzeAsync_WhenAiReturnsInvalidJson_ThrowsException()
        {
            var dreamContent = "Тест сна";
            var invalidJson = "Not a valid JSON { invalid }";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidJson);

            Func<Task> act = async () => await _strategy.AnalyzeAsync(dreamContent);

            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task AnalyzeAsync_WhenAiReturnsEmptyResponse_ThrowsException()
        {
            var dreamContent = "Тест сна";
            var emptyResponse = "";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyResponse);

            Func<Task> act = async () => await _strategy.AnalyzeAsync(dreamContent);

            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task AnalyzeAsync_WhenAiReturnsResponseWithoutBrackets_ThrowsException()
        {
            var dreamContent = "Тест сна";
            var responseWithoutBrackets = "interpretation: test";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ReturnsAsync(responseWithoutBrackets);

            Func<Task> act = async () => await _strategy.AnalyzeAsync(dreamContent);

            await act.Should().ThrowAsync<Exception>().WithMessage("ИИ вернул некорректный формат ответа*");
        }

        [Fact]
        public async Task AnalyzeAsync_WhenAiThrowsException_PropagatesError()
        {
            var dreamContent = "Тест сна";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), dreamContent, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network error"));

            Func<Task> act = async () => await _strategy.AnalyzeAsync(dreamContent);

            await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Network error");
        }
    }
}
