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
            switch (e)
            {
                case EntryPoint.VertexShader:
                    return GetActualNameForMethod(methods.FirstOrDefault(w => w.GetCustomAttribute<VertexShaderAttribute>() != null));
                case EntryPoint.GeometryShader:
                    return GetActualNameForMethod(methods.FirstOrDefault(w => w.GetCustomAttribute<GeometryShaderAttribute>() != null));
                case EntryPoint.FragmentShader:
                    return GetActualNameForMethod(methods.FirstOrDefault(w => w.GetCustomAttribute<FragmentShaderAttribute>() != null));
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }
        }

        private static string GetActualNameForMethod(MethodInfo m)
        {
            var attr = m.GetCustomAttribute<FunctionAttribute>();
            if (attr != null)
                return attr.Alternative;
            return m.Name;
        }
    }
}