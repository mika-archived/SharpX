using System;

using ShaderSharp.Library.Enums.Geometry;

namespace ShaderSharp.Library.Attributes.Geometry
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PrimitiveTypeAttribute : Attribute
    {
        public PrimitiveType Primitive { get; }

        public PrimitiveTypeAttribute(PrimitiveType primitive)
        {
            Primitive = primitive;
        }
    }
}