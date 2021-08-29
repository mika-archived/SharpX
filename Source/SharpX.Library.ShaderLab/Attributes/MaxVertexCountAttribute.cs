using System;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MaxVertexCountAttribute : SourcePartAttribute
    {
        public int VertexCount { get; }

        public MaxVertexCountAttribute(int vertexCount)
        {
            VertexCount = vertexCount;
        }

        public override string ToSourcePart()
        {
            return $"maxvertexcount({VertexCount})";
        }
    }
}