using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class CompilerAnnotatedAttribute : Attribute
    {
        public string Method { get; }

        public CompilerAnnotatedAttribute(string method)
        {
            Method = method;
        }
    }
}