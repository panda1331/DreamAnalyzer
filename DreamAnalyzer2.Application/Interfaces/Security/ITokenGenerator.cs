using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Interfaces.Security
{
    public interface ITokenGenerator
    {
        string GenerateToken(Guid id, string email, string role);
    }
}
