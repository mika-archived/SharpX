using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EarlyDepthStencilAttribute : SourcePartAttribute
    {
        public override string ToSourcePart()
        {
            return "earlydepthstencil";
        }
    }
}