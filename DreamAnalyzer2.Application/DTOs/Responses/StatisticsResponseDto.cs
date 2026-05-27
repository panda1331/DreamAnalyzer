using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.DTOs.Responses
{
    public class StatisticsResponseDto
    {
        public int TotalDreams { get; set; }
        public double AverageDreamLength { get; set; }
        public string? FirstDreamDate { get; set; }
        public string? LastDreamDate { get; set; }
        public Dictionary<string, int> DreamsByWeekDay { get; set; } = new();
        public Dictionary<string, int> MoodDistribution { get; set; } = new();
        public List<PopularSymbolDto> PopularSymbols { get; set; } = new();
    }

    public class PopularSymbolDto
    {
        public string SymbolName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
