using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.Entities
{
    public class DreamSymbol : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public string Interpretation { get; private set; } = string.Empty;
        public string Mood { get; private set;  } = string.Empty;

        protected DreamSymbol() { }
        public DreamSymbol(string name, string interpretation, string mood)
        {
            Name = name;
            Interpretation = interpretation;
            Mood = mood;
        }

        public void Update(string interpretation, string mood)
        {
            Interpretation = interpretation;
            Mood = mood;
        }
    }
}
