using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GlobalMemberAttribute : Attribute
    {
        public string? AlternativeName { get; }

        public GlobalMemberAttribute(string? alternativeName = null)
        {
            AlternativeName = alternativeName;
        }
    }
}