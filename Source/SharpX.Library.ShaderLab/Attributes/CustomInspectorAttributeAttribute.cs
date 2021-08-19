using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CustomInspectorAttributeAttribute : Attribute
    {
        public string Raw { get; }

        public CustomInspectorAttributeAttribute(string raw)
        {
            Raw = raw;
        }
    }
}