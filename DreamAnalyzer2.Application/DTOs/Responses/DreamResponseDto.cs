using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.DTOs.Responses
{
    public class DreamResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime DreamDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public MoodDto? Mood { get; set; }
    }

    public class MoodDto
    {
        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

    }
}
