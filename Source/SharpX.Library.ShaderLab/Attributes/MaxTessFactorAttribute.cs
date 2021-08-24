using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MaxTessFactorAttribute : SourcePartAttribute
    {
        public int Factor { get; }

        public MaxTessFactorAttribute(int factor)
        {
            Factor = factor;
        }

        public override string ToSourcePart()
        {
            return $"maxtessfactor({Factor})";
        }
    }
}