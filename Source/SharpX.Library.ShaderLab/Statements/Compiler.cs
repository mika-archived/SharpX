using System;

using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Statements
{
    [External]
    public static class Compiler
    {
        [CompilerAnnotated(nameof(AnnotatedStatement))]
        public static extern void AnnotatedStatement(string attribute, Action action);

        [CompilerAnnotated(nameof(Raw))]
        public static extern T Raw<T>(string raw);

        [CompilerAnnotated(nameof(Raw))]
        public static extern void Raw(string raw);

    }
}