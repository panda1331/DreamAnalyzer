using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Infrastructure.Repositories
{
    public class DreamRepository : IDreamRepository
    {
        private readonly AppDbContext _context;

        public DreamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Dream dream, CancellationToken cancellationToken = default)
        {
            if (dream == null)
                throw new ArgumentNullException(nameof(dream));
            await  _context.Dreams.AddAsync(dream, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dream = await _context.Dreams.FindAsync(id, cancellationToken);
            if (dream != null)
                _context.Dreams.Remove(dream);
        }

        public async Task<List<Dream>> GetAllAsync()
        {
            return await _context.Dreams.ToListAsync();
        }

        public async Task<Dream?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Dreams
                .Include(d => d.Analysis)
                    .ThenInclude(a => a.Symbols)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<List<Dream>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Dreams
                .Include(d => d.Analysis)
                    .ThenInclude(a => a.Symbols)
                .Where(d => d.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public void Update(Dream dream)
        {
            if (dream == null)
                throw new ArgumentNullException(nameof(dream));
            _context.Dreams.Update(dream);
        }
    }
}
