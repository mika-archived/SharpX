using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CustomInspectorAttributeAttribute : InspectorAttribute
    {
        public string Raw { get; }

        public CustomInspectorAttributeAttribute(string raw)
        {
            Raw = raw;
        }

        public override string ToSourceString()
        {
            return $"{Raw}";
        }
    }
}