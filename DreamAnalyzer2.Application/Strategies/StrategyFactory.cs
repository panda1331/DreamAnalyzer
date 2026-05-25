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

        public StrategyFactory(ISymbolRepository symbolRepository)
        {
            _symbolRepository = symbolRepository;
        }

        public IAnalysisStrategy GetStrategy(string strategyName)
        {
            return strategyName.ToLower() switch
            {
                "symbols" => new SymbolStrategy(_symbolRepository),
                //add strategies...
                _ => throw new ArgumentException($"Unknown strategy: {strategyName}")
            };
        }
    }
}
