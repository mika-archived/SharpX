using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PowerSliderAttribute : InspectorAttribute
    {
        public float Curve { get; }

        public PowerSliderAttribute(float curve)
        {
            Curve = curve;
        }

        public override string ToSourceString()
        {
            return $"PowerSlider({Curve})";
        }
    }
}