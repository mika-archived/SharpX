using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpX.Compiler.Extensions
{
    public static class CSharpSyntaxNodeExtension
    {
        public static bool HasAttribute<T>(this CSharpSyntaxNode obj, SemanticModel model) where T : Attribute
        {
            return obj.GetAttribute<T>(model) != null;
        }

        public static T? GetAttribute<T>(this CSharpSyntaxNode obj, SemanticModel model) where T : Attribute
        {
            var fullyQualifiedMetadataName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(fullyQualifiedMetadataName))
                return null;
            var t = model.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);

            return model.GetDeclaredSymbol(obj)?.GetAttributes().FirstOrDefault(w => w.AttributeClass != null && w.AttributeClass.Equals(t, SymbolEqualityComparer.Default))?.AsAttributeInstance<T>();
        }

        public static string ToLocationString(this CSharpSyntaxNode obj)
        {
            var position = obj.GetLocation().GetLineSpan().StartLinePosition;
            var source = obj.GetLocation().SourceTree?.FilePath ?? "in-memory";
            return $"{source} (Ln {position.Line + 1}, Col {position.Character})";
        }

        public static bool HasInherit<T>(this BaseTypeDeclarationSyntax obj, SemanticModel model)
        {
            return obj.HasInherit(typeof(T), model);
        }

        public static bool HasInherit(this BaseTypeDeclarationSyntax obj, Type s, SemanticModel model)
        {
            var inherits = (obj.BaseList?.Types.Select(w => w.Type).Select(w => model.GetDeclaredSymbol(w)).Where(w => w != null) ?? Array.Empty<ISymbol?>()).ToArray();
            if (inherits.Length == 0)
                return false;

            var t = model.Compilation.GetTypeByMetadataName(s.FullName ?? throw new InvalidOperationException());
            return inherits.Any(w => w?.Equals(t, SymbolEqualityComparer.Default) == true);
        }
    }
}