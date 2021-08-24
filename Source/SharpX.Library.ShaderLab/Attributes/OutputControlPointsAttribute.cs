using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OutputControlPointsAttribute : SourcePartAttribute
    {
        public int ControlPoints { get; }

        public OutputControlPointsAttribute(int controlPoints)
        {
            ControlPoints = controlPoints;
        }

        public override string ToSourcePart()
        {
            return $"outputcontrolpoints({ControlPoints})";
        }
    }
}