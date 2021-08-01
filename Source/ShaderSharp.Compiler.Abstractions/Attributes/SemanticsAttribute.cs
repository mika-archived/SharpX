using System;

namespace ShaderSharp.Compiler.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SemanticsAttribute : Attribute
    {
        public string Semantics { get; }

        public SemanticsAttribute(string semantics)
        {
            Semantics = semantics;
        }
    }
}