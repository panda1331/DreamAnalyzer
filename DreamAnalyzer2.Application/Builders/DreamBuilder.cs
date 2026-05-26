using DreamAnalyzer2.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Application.Builders
{
    public class DreamBuilder
    {
        private Guid _userId;
        private string _title = string.Empty;
        private string _content = string.Empty;
        private DateTime _dreamDate = DateTime.UtcNow;

        public static DreamBuilder Create() => new DreamBuilder();

        public DreamBuilder ForUser(Guid userId)
        {
            _userId = userId;
            return this;
        }
        public DreamBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }
        public DreamBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }
        public DreamBuilder WithDate(DateTime dreamDate)
        {
            _dreamDate = dreamDate;
            return this;
        }
        public Dream Build()
        {
            return new Dream(_userId, _title, _content, _dreamDate);
        }
    }
}
