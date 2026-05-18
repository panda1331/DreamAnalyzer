using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Shared.Shared
{
    public class ValidationException : Exception
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
    }
}
