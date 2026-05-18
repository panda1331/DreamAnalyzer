using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Interfaces.Security
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
