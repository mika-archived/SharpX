using System;

using SharpX.Library.ShaderLab.Attributes.Internal;
using SharpX.Library.ShaderLab.Enums;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OutputTopologyAttribute : SourcePartAttribute
    {
        public OutputTopology OutputTopology { get; }

        public OutputTopologyAttribute(OutputTopology outputTopology)
        {
            OutputTopology = outputTopology;
        }

        public override string ToSourcePart()
        {
            var topology = OutputTopology switch
            {
                OutputTopology.Point => "point",
                OutputTopology.Line => "line",
                OutputTopology.TriangleCw => "triangle_cw",
                OutputTopology.TriangleCcw => "triangle_ccw",
                _ => throw new ArgumentOutOfRangeException()
            };

            return $"outputtopology(\"{topology}\")";
        }
    }
}