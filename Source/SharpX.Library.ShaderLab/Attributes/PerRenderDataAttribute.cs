using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PerRenderDataAttribute : InspectorAttribute
    {
        public override string ToSourceString()
        {
            return "PerRenderData";
        }
    }
}