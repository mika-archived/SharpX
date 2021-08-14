using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class InputPrimitiveAttribute : Attribute
    {
        public enum InputPrimitives
        {
            Point,

            Line,

            Triangle,

            LineAdj,

            TriangleAdj
        }

        public InputPrimitives Primitives { get; }

        public InputPrimitiveAttribute(InputPrimitives primitives)
        {
            Primitives = primitives;
        }
    }
}