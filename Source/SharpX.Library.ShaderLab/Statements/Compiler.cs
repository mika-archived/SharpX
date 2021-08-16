using System;

using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Statements
{
    [External]
    public static class Compiler
    {
        [CompilerAnnotated("Annotated")]
        public static extern void AnnotatedStatement(string attribute, Action action);

        [CompilerAnnotated("RawInput")]
        public static extern void Raw(string raw);

    }
}