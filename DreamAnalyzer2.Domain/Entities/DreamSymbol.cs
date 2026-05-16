using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.Entities
{
    public class DreamSymbol
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        //private readonly List<DreamAnalysis> _analyses = new();
        //public IReadOnlyCollection<DreamAnalysis> Analyses => _analyses.AsReadOnly();

        protected DreamSymbol() { }
        public DreamSymbol(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
