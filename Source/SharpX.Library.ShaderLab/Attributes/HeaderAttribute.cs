using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class HeaderAttribute : InspectorAttribute
    {
        public string Header { get; }

        public HeaderAttribute(string header)
        {
            Header = header;
        }

        public override string ToSourceString()
        {
            return $"Header({Header})";
        }
    }
}