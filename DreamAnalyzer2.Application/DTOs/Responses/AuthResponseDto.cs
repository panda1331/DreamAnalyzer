using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.DTOs.Responses
{
    public class AuthResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
