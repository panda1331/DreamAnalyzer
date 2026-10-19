using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Shared.Shared
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
