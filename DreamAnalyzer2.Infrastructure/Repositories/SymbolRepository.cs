using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Infrastructure.Repositories
{
    public class SymbolRepository : ISymbolRepository
    {
        private readonly AppDbContext _context;

        public SymbolRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DreamSymbol dreamSymbol)
        {
            await _context.DreamSymbols.AddAsync(dreamSymbol);
        }

        public async Task DeleteAsync(Guid id)
        {
            var symbol = await GetByIdAsync(id);
            if (symbol != null)
                _context.DreamSymbols.Remove(symbol);
        }

        public async Task<List<DreamSymbol>> GetAllAsync()
        {
            return await _context.DreamSymbols.ToListAsync();
        }

        public async Task<DreamSymbol?> GetByIdAsync(Guid id)
        {
            return await _context.DreamSymbols.FindAsync(id);
        }

        public async Task<DreamSymbol?> GetByNameAsync(string name)
        {
            return await _context.DreamSymbols.Where(s => s.Name == name).FirstOrDefaultAsync();
        }

        public void Update(DreamSymbol dreamSymbol)
        {
            _context.DreamSymbols.Update(dreamSymbol);
        }
    }
}
