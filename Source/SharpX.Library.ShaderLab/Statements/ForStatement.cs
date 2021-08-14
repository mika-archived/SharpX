using System;

using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Statements
{
    [External]
    public static class ForStatement
    {
        [CompilerAnnotated]
        public static extern void AttributedFor(string attribute, Action action);
    }
}