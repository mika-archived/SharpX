using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GlobalMemberAttribute : Attribute
    {
        public bool IsNotDeclareInSource { get; }

        public GlobalMemberAttribute(bool isNotDeclareInSource = false)
        {
            IsNotDeclareInSource = isNotDeclareInSource;
        }
    }
}