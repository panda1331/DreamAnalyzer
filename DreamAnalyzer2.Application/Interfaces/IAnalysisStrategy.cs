using DreamAnalyzer2.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Interfaces
{
    public interface IAnalysisStrategy
    {
        Task<AnalysisResponseDto> AnalyzeAsync(string content);
    }
}
