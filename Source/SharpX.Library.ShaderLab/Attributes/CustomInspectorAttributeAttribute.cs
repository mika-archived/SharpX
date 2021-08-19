using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CustomInspectorAttributeAttribute : Attribute
    {
        public string Raw { get; }

        public object[] Parameters { get; }

        public CustomInspectorAttributeAttribute(string raw, params object[] parameters)
        {
            Raw = raw;
            Parameters = parameters;
        }
    }
}