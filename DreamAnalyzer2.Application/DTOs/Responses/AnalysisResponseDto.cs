using DreamAnalyzer2.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.DTOs.Responses
{
    public class AnalysisResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Interpretation { get; set; } = string.Empty;
        public Mood Mood { get; set; }
        public List<string> Symbols { get; set; } = new();
        public string Strategy { get; set; } = string.Empty;
    }
}
