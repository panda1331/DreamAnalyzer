using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Shared.Shared
{
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
    }
}
