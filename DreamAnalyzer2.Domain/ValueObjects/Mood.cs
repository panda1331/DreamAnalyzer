using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.ValueObjects
{
    public record Mood
    {
        public string Name { get; init; } = string.Empty;
        public string ColorHex { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;

        public Mood(string name, string colorHex, string description)
        {
            Name = name;
            ColorHex = colorHex;
            Description = description;
        }

        public static Mood Peaceful => new Mood("Peaceful", "#378c3a", "Dream brings peace and harmony.");
        public static Mood Anxious => new Mood("Anxious", "#b80909", "Sleep is accompanied by a feeling of uncertainty or mild fear.");
        public static Mood NightMare => new Mood("NightMare", "#0b0738", "Intense fear, terror, panic. It often leads to waking up.");
        public static Mood Lucid => new Mood("Lucid", "#9C27B0", "The user understood what was inside the dream.");

        public static Mood Create(string name, string colorHex = "#016156", string description = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Dream mood name can't be empty.");
            return new Mood(name, colorHex, description);
        }
    }
}
