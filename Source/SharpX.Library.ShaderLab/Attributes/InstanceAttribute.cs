using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InstanceAttribute : SourcePartAttribute
    {
        public int X { get; }

        public InstanceAttribute(int x)
        {
            X = x;
        }

        public override string ToSourcePart()
        {
            return $"instance({X})";
        }
    }
}