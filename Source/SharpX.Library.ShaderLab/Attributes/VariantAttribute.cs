using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class VariantAttribute : Attribute
    {
        public string Variant { get; }

        public VariantAttribute(string variant)
        {
            Variant = variant;
        }
    }
}