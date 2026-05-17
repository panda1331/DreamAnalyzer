using DreamAnalyzer2.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.Interfaces
{
    public interface IDreamRepository
    {
        Task<Dream?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Dream>> GetByUserId(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Dream dream, CancellationToken cancellationToken = default);
        Task UpdateAsync(Dream dream, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
