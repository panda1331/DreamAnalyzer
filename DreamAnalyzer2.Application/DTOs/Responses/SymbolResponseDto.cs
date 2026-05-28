using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.DTOs.Responses
{
    public class SymbolResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Interpretation { get; set; } = string.Empty;
        public string Mood { get; set; } = string.Empty;
    }
}
