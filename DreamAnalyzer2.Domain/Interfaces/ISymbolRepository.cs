using DreamAnalyzer2.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.Interfaces
{
    public interface ISymbolRepository
    {
        Task<List<DreamSymbol>> GetAllAsync();
        Task<DreamSymbol?> GetByNameAsync(string name);
        Task<DreamSymbol?> GetByIdAsync(Guid id);
        Task AddAsync(DreamSymbol dreamSymbol);
        void Update(DreamSymbol dreamSymbol);
        Task DeleteAsync(Guid id);
    }
}
