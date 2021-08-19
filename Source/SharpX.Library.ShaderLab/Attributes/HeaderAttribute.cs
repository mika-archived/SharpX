using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class HeaderAttribute : Attribute
    {
        public string Header { get; }

        public HeaderAttribute(string header)
        {
            Header = header;
        }
    }
}