using DreamAnalyzer2.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace DreamAnalyzer2.Domain.Entities
{
    public class DreamAnalysis : Entity
    {
        public Guid DreamId { get; private set; }
        public string Interpretation { get; private set; } = string.Empty;
        public Mood Mood { get; private set; }

        private readonly List<DreamSymbol> _symbols = new();
        public IReadOnlyCollection<DreamSymbol> Symbols => _symbols.AsReadOnly();

        protected DreamAnalysis() { }
        public DreamAnalysis(Guid dreamId, string interpretation, Mood mood)
        {
            DreamId = dreamId;
            Interpretation = interpretation;
            Mood = mood ?? throw new ArgumentNullException(nameof(mood));
        }

        public void AddSymbol(DreamSymbol symbol)
        {
            if (symbol == null) throw new ArgumentNullException(nameof(symbol));
            if (!_symbols.Contains(symbol))
            {
                _symbols.Add(symbol);
            }
        }
    }
}
