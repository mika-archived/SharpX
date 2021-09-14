using System;

using SharpX.Library.ShaderLab.Enums;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InterpolationModifierAttribute : Attribute
    {
        public InterpolationModifier Modifier { get; }

        public InterpolationModifierAttribute(InterpolationModifier modifier = InterpolationModifier.Linear)
        {
            Modifier = modifier;
        }

        public string ToModifierString()
        {
            return Modifier switch
            {
                InterpolationModifier.Linear => "",
                InterpolationModifier.LinearCentroid => "linear centroid",
                InterpolationModifier.NoInterpolation => "nointerpolation",
                InterpolationModifier.NoPerspective => "noperspective",
                InterpolationModifier.NoPerspectiveCentroid => "noperspective centroid",
                InterpolationModifier.Sample => "sample",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}