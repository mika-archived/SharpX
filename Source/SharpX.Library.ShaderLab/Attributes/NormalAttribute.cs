using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NormalAttribute : InspectorAttribute
    {
        public override string ToSourceString()
        {
            return "Normal";
        }
    }
}