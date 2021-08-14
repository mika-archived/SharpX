using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MaxVertexCountAttribute : Attribute
    {
        public int VertexCount { get; }

        public MaxVertexCountAttribute(int vertexCount)
        {
            VertexCount = vertexCount;
        }
    }
}