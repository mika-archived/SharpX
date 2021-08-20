using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NoScaleOffsetAttribute : InspectorAttribute
    {
        public override string ToSourceString()
        {
            return "NoScaleOffset";
        }
    }
}