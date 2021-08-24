using System;
using System.Linq;
using System.Reflection;

using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Abstractions
{
    public static class Configuration
    {
        public enum EntryPoint
        {
            VertexShader,

            GeometryShader,

            FragmentShader
        }

        public static string GetShaderEntryPoint(Type t, EntryPoint e)
        {
            var methods = t.GetMethods();
            return e switch
            {
                EntryPoint.VertexShader => GetActualNameForMethod(methods.FirstOrDefault(w => w.GetCustomAttribute<VertexShaderAttribute>() != null)),
                EntryPoint.GeometryShader => GetActualNameForMethod(methods.FirstOrDefault(w => w.GetCustomAttribute<GeometryShaderAttribute>() != null)),
                EntryPoint.FragmentShader => GetActualNameForMethod(methods.FirstOrDefault(w => w.GetCustomAttribute<FragmentShaderAttribute>() != null)),
                _ => throw new ArgumentOutOfRangeException(nameof(e), e, null)
            };
        }

        private static string GetActualNameForMethod(MethodInfo? m)
        {
            if (m == null)
                return "/* UNKNOWN */";

            var attr = m.GetCustomAttribute<FunctionAttribute>();
            return attr != null ? attr.Alternative : m.Name;
        }
    }
}