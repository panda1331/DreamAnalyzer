using DreamAnalyzer2.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Interfaces.Services
{
    public interface IAnalysisService
    {
        Task<AnalysisResponseDto> AnalyseDreamAsync(Guid userId, Guid id, IAnalysisStrategy strategy);
    }
}
