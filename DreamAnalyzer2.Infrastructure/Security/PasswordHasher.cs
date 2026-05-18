using BCrypt.Net;
using DreamAnalyzer2.Application.Interfaces.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
