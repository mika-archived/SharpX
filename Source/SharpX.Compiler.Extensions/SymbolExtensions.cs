using System;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;

namespace SharpX.Compiler.Extensions
{
    public static class SymbolExtensions
    {
        public static T? GetAttribute<T>(this ISymbol? obj, SemanticModel model) where T : Attribute
        {
            var fullyQualifiedMetadataName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(fullyQualifiedMetadataName))
                return default;

            var t = model.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
            if (t == null)
                return default;

            return obj?.GetAttributes().FirstOrDefault(w => w.AttributeClass != null && w.AttributeClass.Equals(t, SymbolEqualityComparer.Default))?.AsAttributeInstance<T>();
        }

        public static bool HasAttribute(this ISymbol? obj, string fullyQualifiedMetadataName, SemanticModel model)
        {
            var t = model.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
            if (t == null)
                return false;

            return obj?.GetAttributes().Any(w => w.AttributeClass != null && w.AttributeClass.Equals(t, SymbolEqualityComparer.Default)) == true;
        }

        public static bool HasAttribute<T>(this ISymbol? obj, SemanticModel model) where T : Attribute
        {
            var fullyQualifiedMetadataName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(fullyQualifiedMetadataName))
                return default;

            var t = model.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
            if (t == null)
                return false;

            return obj?.GetAttributes().Any(w => w.AttributeClass != null && w.AttributeClass.Equals(t, SymbolEqualityComparer.Default)) == true;
        }

        public static string ToUniqueSignature(this ISymbol obj)
        {
            var sb = new StringBuilder();
            sb.Append(obj.ContainingSymbol.ToDisplayString());
            sb.Append("#");
            sb.Append(obj.Name);

            return sb.ToString();
        }
    }
}