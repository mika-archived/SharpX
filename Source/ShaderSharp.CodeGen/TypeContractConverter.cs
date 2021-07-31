using System;

using ShaderSharp.CodeGen.Models;

namespace ShaderSharp.CodeGen
{
    public static class TypeContractConverter
    {
        private const string PrimitiveNamespace = "ShaderSharp.Library.Primitives";
        private const string AbstractionsNamespace = "ShaderSharp.Library.Primitives.Abstractions";

        public static string GetAsCSharpTypeReference(Parameter parameter, int e = 1)
        {
            switch (parameter.Component)
            {
                case "":
                case null:
                    return parameter.Type;

                case "scalar":
                    return GetAsCSharpScalarComponentTypeReference(parameter);

                case "vector":
                    return GetAsCSharpVectorComponentTypeReference(parameter, e);

                case "matrix":
                    return "object";

                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter.Component));
            }
        }

        public static bool ShouldUseGenericDefinition(Parameter parameter)
        {
            var t = GetAsCSharpTypeReference(parameter);
            return t.StartsWith(AbstractionsNamespace);
        }

        public static int CalculateOverloadCount(Parameter parameter)
        {
            switch (parameter.Component)
            {
                case "scalar":
                    return 1;

                case "vector" when string.IsNullOrWhiteSpace(parameter.Element):
                    return 3;

                case "vector" when int.TryParse(parameter.Element, out _):
                    return 1;

                case "matrix" when string.IsNullOrWhiteSpace(parameter.Element):
                    return 0; // 16;

                case "matrix" when int.TryParse(parameter.Element, out _):
                    return 0; // 1;

                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter.Component));
            }
        }

        private static string GetAsCSharpScalarComponentTypeReference(Parameter parameter)
        {
            int.TryParse(parameter.Element, out var element);

            switch (parameter.Type)
            {
                case "bool":
                    return $"{PrimitiveNamespace}.SlBool{(element > 1 ? element : "")}";

                case "float":
                    return $"{PrimitiveNamespace}.SlFloat{(element > 1 ? element : "")}";

                case "int":
                    return $"{PrimitiveNamespace}.SlInt{(element > 1 ? element : "")}";

                case "uint":
                    return $"{PrimitiveNamespace}.SlUint{(element > 1 ? element : "")}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter.Type));
            }
        }

        private static string GetAsCSharpVectorComponentTypeReference(Parameter parameter, int e = 1)
        {
            switch (parameter.Type)
            {
                case "bool" when string.IsNullOrWhiteSpace(parameter.Element):
                    return $"{PrimitiveNamespace}.SlBool{e}";

                case "bool" when int.TryParse(parameter.Element, out var element):
                    return $"{PrimitiveNamespace}.SlBool{element}";

                case "float" when string.IsNullOrWhiteSpace(parameter.Element):
                    return $"{PrimitiveNamespace}.SlFloat{e}";

                case "float" when int.TryParse(parameter.Element, out var element):
                    return $"{PrimitiveNamespace}.SlFloat{element}";

                case "int" when string.IsNullOrWhiteSpace(parameter.Element):
                    return $"{PrimitiveNamespace}.SlInt{e}";

                case "int" when int.TryParse(parameter.Element, out var element):
                    return $"{PrimitiveNamespace}.SlInt{element}";

                case "uint" when string.IsNullOrWhiteSpace(parameter.Element):
                    return $"{PrimitiveNamespace}.SlUint{e}";

                case "uint" when int.TryParse(parameter.Element, out var element):
                    return $"{PrimitiveNamespace}.SlUint{element}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter.Type));
            }
        }
    }
}