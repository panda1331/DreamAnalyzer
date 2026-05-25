using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Infrastructure.Repositories
{
    public class AnalysisRepository : IAnalysisRepository
    {
        private readonly AppDbContext _appDbContext;

        public AnalysisRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddAsync(DreamAnalysis analysis)
        {
            await _appDbContext.DreamAnalyses.AddAsync(analysis);
        }
    }
}
