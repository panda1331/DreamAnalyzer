using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Domain.Entities
{
    public class Dream : Entity
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime DreamDate { get; private set; }
        public DreamAnalysis? Analysis { get; private set; }

        protected Dream() { }

        public Dream(Guid userId,
                        string title,
                        string content,
                        DateTime dreamdate)
        {
            UserId = userId;
            Title = title;
            Content = content;
            CreatedAt = DateTime.UtcNow;
            DreamDate = dreamdate;
        }

        public void SetAnalysis(DreamAnalysis analysis)
        {
            if (analysis == null) throw new ArgumentNullException(nameof(analysis));
            Analysis = analysis;
        }
    }
}
