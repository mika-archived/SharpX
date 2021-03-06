using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class RawAttribute : Attribute
    {
        public string Raw { get; }

        public RawAttribute(string raw)
        {
            Raw = raw;
        }
    }
}