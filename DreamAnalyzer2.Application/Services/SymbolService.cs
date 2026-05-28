using DreamAnalyzer2.Application.DTOs.Requests;
using DreamAnalyzer2.Application.DTOs.Requests.Dreams;
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
    public class SymbolService : ISymbolService
    {
        private readonly ISymbolRepository _symbolRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SymbolService(ISymbolRepository symbolRepository, IUnitOfWork unitOfWork)
        {
            _symbolRepository = symbolRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SymbolResponseDto> CreateSymbolAsync(CreateSymbolDto creatSymbolDto)
        {
            var symbol = new DreamSymbol(creatSymbolDto.Name, creatSymbolDto.Interpretation, creatSymbolDto.Mood);
            await _symbolRepository.AddAsync(symbol);
            await _unitOfWork.SaveChangesAsync();

            return new SymbolResponseDto
            {
                Id = symbol.Id,
                Name = symbol.Name,
                Interpretation = symbol.Interpretation,
                Mood = symbol.Mood,
            };
        }

        public async Task DeleteSymbolAsync(Guid id)
        {
            await _symbolRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<SymbolResponseDto>> GetAllSymbolsAsync()
        {
            var symbols = await _symbolRepository.GetAllAsync();
            var responses = new List<SymbolResponseDto>();
            foreach (var symbol in symbols)
                responses.Add(ToResponse(symbol));
            return responses;
        }

        public async Task<SymbolResponseDto> GetSymbolByIdAsync(Guid id)
        {
            var symbol = await _symbolRepository.GetByIdAsync(id);
            if (symbol == null)
                throw new NotFoundException("Symbol not found");
            return ToResponse(symbol);
        }

        public async Task<SymbolResponseDto> UpdateSymbolAsync(Guid id, UpdateSymbolDto updateSymbolDto)
        {
            var symbol = await _symbolRepository.GetByIdAsync(id);
            if (symbol == null)
                throw new NotFoundException("Symbol not found");

            symbol.Update(updateSymbolDto.Interpretation, updateSymbolDto.Mood);
            _symbolRepository.Update(symbol);
            await _unitOfWork.SaveChangesAsync();

            return ToResponse(symbol);
        }

        private SymbolResponseDto ToResponse(DreamSymbol symbol)
        {
            return new SymbolResponseDto
            {
                Id = symbol.Id,
                Name = symbol.Name,
                Interpretation = symbol.Interpretation,
                Mood = symbol.Mood,
            };
        }
    }
}
