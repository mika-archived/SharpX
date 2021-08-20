using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    // ReSharper disable once InconsistentNaming
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class HDRAttribute : InspectorAttribute
    {
        public override string ToSourceString()
        {
            return "HDR";
        }
    }
}