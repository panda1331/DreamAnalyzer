using DreamAnalyzer2.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        void Update(User user);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
