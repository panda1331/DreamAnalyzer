using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.DTOs.Responses
{
    public class ProfileResponseDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegistryDate { get; set; }
        public int DreamsCount { get; set; }
    }
}
