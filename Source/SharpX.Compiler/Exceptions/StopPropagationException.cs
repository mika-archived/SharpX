using System;

namespace SharpX.Compiler.Exceptions
{
    internal class StopPropagationException : Exception
    {
        public StopPropagationException() { }

        public StopPropagationException(string? message) : base(message) { }

        public StopPropagationException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}