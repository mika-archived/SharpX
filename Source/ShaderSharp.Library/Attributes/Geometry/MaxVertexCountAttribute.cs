using System;

namespace ShaderSharp.Library.Attributes.Geometry
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MaxVertexCountAttribute : Attribute
    {
        public int NumVerts { get; }

        public MaxVertexCountAttribute(int numVerts)
        {
            NumVerts = numVerts;
        }
    }
}