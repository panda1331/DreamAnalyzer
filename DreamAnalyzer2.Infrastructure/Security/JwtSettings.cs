using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Infrastructure.Security
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryHours { get; set; }

    }
}
