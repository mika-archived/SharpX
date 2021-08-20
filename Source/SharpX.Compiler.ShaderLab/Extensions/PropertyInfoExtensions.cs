using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharpX.Compiler.ShaderLab.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static dynamic? GetCustomSameAttribute<T>(this PropertyInfo obj)
        {
            var fullyQualifiedName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(fullyQualifiedName))
                return null;

            var attributes = obj.GetCustomAttributes(false);
            return attributes.FirstOrDefault(w => w.GetType().FullName == fullyQualifiedName);
        }
    }
}