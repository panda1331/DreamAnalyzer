using DreamAnalyzer2.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.Interfaces
{
    public interface IAnalysisRepository
    {
        Task AddAsync(DreamAnalysis analysis);
    }
}
