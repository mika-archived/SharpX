using System;
using System.Linq;

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
    }
}