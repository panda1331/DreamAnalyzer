using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.DTOs.Responses
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string Role {  get; set; } = string.Empty;
    }
}
