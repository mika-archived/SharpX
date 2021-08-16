using System;

using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Statements
{
    [External]
    public static class Annotated
    {
        [CompilerAnnotated]
        public static extern void AnnotatedStatement(string attribute, Action action);
    }
}