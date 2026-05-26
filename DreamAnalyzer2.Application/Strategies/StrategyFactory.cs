using DreamAnalyzer2.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using DreamAnalyzer2.Domain.Interfaces;


namespace DreamAnalyzer2.Application.Strategies
{
    public class StrategyFactory : IStrategyFactory
    {
        private readonly ISymbolRepository _symbolRepository;
        private readonly IDreamAiClient _aiClient;

        public StrategyFactory(ISymbolRepository symbolRepository, IDreamAiClient aiClient)
        {
            _symbolRepository = symbolRepository;
            _aiClient = aiClient;
        }

        public IAnalysisStrategy GetStrategy(string strategyName, string? book = null)
        {
            return strategyName.ToLower() switch
            {
                "symbols" => new SymbolStrategy(_symbolRepository),
                "freudian" => new FreudianAnalysisStrategy(_aiClient),
                "jungian" => new JungianAnalysisStrategy(_aiClient),
                "cognitive" => new CognitiveAnalysisStrategy(_aiClient),
                "dreambook" => new DreamBookAnalysisStrategy(_aiClient, book),
                _ => throw new ArgumentException($"Unknown strategy: {strategyName}")
            };
        }
    }
}
