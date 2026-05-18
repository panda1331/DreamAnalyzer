using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
