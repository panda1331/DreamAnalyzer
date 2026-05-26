using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Interfaces.Services;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Domain.ValueObjects;
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
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AnalysisService(ISymbolRepository symbolRepository, IDreamRepository dreamRepository, IUnitOfWork unitOfWork, IAnalysisRepository analysisRepository)
        {
            _symbolRepository = symbolRepository;
            _dreamRepository = dreamRepository;
            _unitOfWork = unitOfWork;
            _analysisRepository = analysisRepository;
        }

        public async Task<AnalysisResponseDto> AnalyseDreamAsync(Guid userId, Guid id, IAnalysisStrategy strategy)
        {
            var dream = await _dreamRepository.GetByIdAsync(id);
            if (dream == null)
                throw new NotFoundException("No such dream");
            if (dream.UserId != userId)
                throw new ValidationException("You can analyze only your dreams");

            string contentToAnalyze = dream.Content;
            var strategyResult = await strategy.AnalyzeAsync(dream.Content);

            var domainMood = Mood.Create(strategyResult.MoodName);

            DreamAnalysis analysis;
            if (dream.Analysis != null)
            {
                analysis = dream.Analysis;
                analysis.Update(strategyResult.Interpretation, domainMood);
                analysis.ClearSymbols();
            }
            else
            {
                analysis = new DreamAnalysis(dream.Id, strategyResult.Interpretation, domainMood);
                await _analysisRepository.AddAsync(analysis);
                dream.SetAnalysis(analysis);
            }

            await _unitOfWork.SaveChangesAsync();

            foreach (var symbolName in strategyResult.Symbols)
            {
                var symbol = await _symbolRepository.GetByNameAsync(symbolName);
                if (symbol != null)
                    analysis.AddSymbol(symbol);
            }
            await _unitOfWork.SaveChangesAsync();

            return new AnalysisResponseDto
            {
                Title = dream.Title,
                Interpretation = strategyResult.Interpretation,
                MoodName = strategyResult.MoodName,
                MoodColor = strategyResult.MoodColor,
                MoodDescription = strategyResult.MoodDescription,
                Symbols = strategyResult.Symbols,
                Strategy = strategyResult.Strategy,
            };
        }
    }
}
