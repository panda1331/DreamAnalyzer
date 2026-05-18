using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Shared.Shared
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() { }
        public InvalidCredentialsException(string message) : base(message) { }
    }
}
