using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Strategies;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Tests.Strategies
{
    public class DreamBookAnalysisStrategyTests
    {
        private readonly Mock<IDreamAiClient> _aiClientMock;

        public DreamBookAnalysisStrategyTests()
        {
            _aiClientMock = new Mock<IDreamAiClient>();
        }

        [Theory]
        [InlineData("miller", "Сонник Миллера")]
        [InlineData("vanga", "Сонник Ванги")]
        [InlineData("nostradamus", "Сонник Нострадамуса")]
        public async Task AnalyzeAsync_DifferentBooks_ShouldReturnCorrectTitle(string book, string expectedTitleStart)
        {
            // Arrange
            var strategy = new DreamBookAnalysisStrategy(_aiClientMock.Object, book);
            var aiResponse = "{\"interpretation\": \"Толкование сна\", \"mood\": \"Peaceful\"}";

            _aiClientMock
                .Setup(x => x.GetJsonCompletionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(aiResponse);

            // Act
            var result = await strategy.AnalyzeAsync("Тест сна");

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().StartWith(expectedTitleStart);
        }
    }
}
