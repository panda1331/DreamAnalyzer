using DreamAnalyzer2.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.Interfaces
{
    public interface IDreamRepository
    {
        Task<List<Dream>> GetAllAsync();
        Task<Dream?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Dream>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Dream dream, CancellationToken cancellationToken = default);
        void Update(Dream dream);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
