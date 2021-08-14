using System;

using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Extensions
{
    internal static class InputPrimitivesExtensions
    {
        public static int GetArrayElement(this InputPrimitiveAttribute.InputPrimitives obj)
        {
            switch (obj)
            {
                case InputPrimitiveAttribute.InputPrimitives.Point:
                    return 1;

                case InputPrimitiveAttribute.InputPrimitives.Line:
                    return 2;

                case InputPrimitiveAttribute.InputPrimitives.Triangle:
                    return 3;

                case InputPrimitiveAttribute.InputPrimitives.LineAdj:
                    return 4;

                case InputPrimitiveAttribute.InputPrimitives.TriangleAdj:
                    return 6;

                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }
    }
}