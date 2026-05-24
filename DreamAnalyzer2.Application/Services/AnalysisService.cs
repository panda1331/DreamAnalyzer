using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Interfaces.Services;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Shared.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly ISymbolRepository _symbolRepository;
        private readonly IDreamRepository _dreamRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AnalysisService(ISymbolRepository symbolRepository, IDreamRepository dreamRepository, IUnitOfWork unitOfWork)
        {
            _symbolRepository = symbolRepository;
            _dreamRepository = dreamRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AnalysisResponseDto> AnalyseDreamAsync(Guid userId, Guid id, IAnalysisStrategy strategy)
        {
            var dream = await _dreamRepository.GetByIdAsync(id);
            if (dream == null)
                throw new NotFoundException("No such dream");
            if (dream.UserId != userId)
                throw new ValidationException("You can analyze only your dreams");

            var strategyResult = await strategy.AnalyzeAsync(dream.Content);
            var analysis = new DreamAnalysis(dream.Id, strategyResult.Interpretation, strategyResult.Mood);

            foreach (var symbolName in strategyResult.Symbols)
            {
                var symbol = await _symbolRepository.GetByNameAsync(symbolName);
                if (symbol != null)
                    analysis.AddSymbol(symbol);
            }
            dream.SetAnalysis(analysis);
            _dreamRepository.Update(dream);
            await _unitOfWork.SaveChangesAsync();
            return new AnalysisResponseDto
            {
                Title = dream.Title,
                Interpretation = strategyResult.Interpretation,
                Mood = strategyResult.Mood,
                Symbols = strategyResult.Symbols,
                Strategy = strategyResult.Strategy,
            };
        }
    }
}
