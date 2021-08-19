using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PowerSliderAttribute : Attribute
    {
        public float Curve { get; }

        public PowerSliderAttribute(float curve)
        {
            Curve = curve;
        }
    }
}