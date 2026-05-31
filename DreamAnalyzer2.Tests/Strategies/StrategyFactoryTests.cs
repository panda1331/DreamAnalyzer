using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Strategies;
using DreamAnalyzer2.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Tests.Strategies
{
    public class StrategyFactoryTests
    {
        private readonly Mock<ISymbolRepository> _symbolRepositoryMock;
        private readonly Mock<IDreamAiClient> _aiClientMock;
        private readonly StrategyFactory _factory;

        public StrategyFactoryTests()
        {
            _symbolRepositoryMock = new Mock<ISymbolRepository>();
            _aiClientMock = new Mock<IDreamAiClient>();
            _factory = new StrategyFactory(_symbolRepositoryMock.Object, _aiClientMock.Object);
        }

        [Fact]
        public void GetStrategy_WithSymbolsStrategy_ReturnsSymbolStrategy()
        {
            var strategy = _factory.GetStrategy("symbols");
            strategy.Should().NotBeNull();
            strategy.Should().BeOfType<SymbolStrategy>();
        }

        [Fact]
        public void GetStrategy_WithSymbolsStrategyCaseInsensitive_ReturnsSymbolStrategy()
        {
            var strategy = _factory.GetStrategy("SYMBOLS");
            strategy.Should().NotBeNull();
            strategy.Should().BeOfType<SymbolStrategy>();
        }

        [Fact]
        public void GetStrategy_WithFreudianStrategy_ReturnsFreudianAnalysisStrategy()
        {
            var strategy = _factory.GetStrategy("freudian");
            strategy.Should().NotBeNull();
            strategy.Should().BeOfType<FreudianAnalysisStrategy>();
        }

        [Fact]
        public void GetStrategy_WithJungianStrategy_ReturnsJungianAnalysisStrategy()
        {
            var strategy = _factory.GetStrategy("jungian");
            strategy.Should().NotBeNull();
            strategy.Should().BeOfType<JungianAnalysisStrategy>();
        }

        [Fact]
        public void GetStrategy_WithCognitiveStrategy_ReturnsCognitiveAnalysisStrategy()
        {
            var strategy = _factory.GetStrategy("cognitive");
            strategy.Should().NotBeNull();
            strategy.Should().BeOfType<CognitiveAnalysisStrategy>();
        }

        [Fact]
        public void GetStrategy_WithDreamBookStrategy_ReturnsDreamBookAnalysisStrategy()
        {
            var strategy = _factory.GetStrategy("dreambook");
            strategy.Should().NotBeNull();
            strategy.Should().BeOfType<DreamBookAnalysisStrategy>();
        }

        [Fact]
        public void GetStrategy_WithDreamBookAndBookParameter_PassesBookToStrategy()
        {
            var strategy = _factory.GetStrategy("dreambook", "vanga");
            strategy.Should().NotBeNull();
            strategy.Should().BeOfType<DreamBookAnalysisStrategy>();
        }

        [Fact]
        public void GetStrategy_WithUnknownStrategy_ThrowsArgumentException()
        {
            Action act = () => _factory.GetStrategy("unknown");

            act.Should().Throw<ArgumentException>()
                .WithMessage("Unknown strategy: unknown");
        }

        [Fact]
        public void GetStrategy_WithEmptyStrategyName_ThrowsArgumentException()
        {
            Action act = () => _factory.GetStrategy("");
            act.Should().Throw<ArgumentException>();
        }
    }
}
