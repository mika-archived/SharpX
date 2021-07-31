using System;

namespace ShaderSharp.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SemanticsAttribute : Attribute
    {
        public string Semantics { get; }

        public SemanticsAttribute(string semantics)
        {
            Semantics = semantics;
        }
    }
}