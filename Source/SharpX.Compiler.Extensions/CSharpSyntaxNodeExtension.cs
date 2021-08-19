using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

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
            var t = model.Compilation.GetTypeByMetadataName(s.FullName ?? throw new InvalidOperationException());

            var symbol = model.GetDeclaredSymbol(obj);

            var inherit = symbol;
            while (true)
            {
                if (inherit?.Equals(t, SymbolEqualityComparer.Default) == true)
                    return true;

                if (inherit?.BaseType == null)
                    break;

                inherit = inherit.BaseType;
            }

            var i = symbol?.AllInterfaces ?? ImmutableArray<INamedTypeSymbol>.Empty;
            return i.Any(w => w.Equals(t, SymbolEqualityComparer.Default));
        }

        public static bool HasModifiers(this MemberDeclarationSyntax obj, SyntaxKind token)
        {
            return obj.Modifiers.Any(w => w.Kind() == token);
        }

        public static T? AsAttributeInstance<T>(this AttributeSyntax obj, SemanticModel model)
        {
            var fullyQualifiedMetadataName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(fullyQualifiedMetadataName))
                return default;

            var n = model.GetSymbolInfo(obj);
            if (n.Symbol?.ContainingType?.Equals(model.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName), SymbolEqualityComparer.Default) != true)
                return default;

            var t = typeof(T);
            var constructors = t.GetConstructors();
            var constructorArguments = obj.ArgumentList?.Arguments.Where(w => w.NameEquals == null).ToList() ?? new List<AttributeArgumentSyntax>();
            var namedArguments = obj.ArgumentList?.Arguments.Where(w => w.NameEquals != null).ToList() ?? new List<AttributeArgumentSyntax>();

            // TODO: Type Checking
            var constructor = constructors.FirstOrDefault(w => w.GetParameters().Length == constructorArguments.Count);
            if (constructor == null)
                return default;

            var instance = (T) constructor.Invoke(constructorArguments.Select(w => model.GetConstantValue(w.Expression).Value).ToArray());

            if (namedArguments.Count == 0)
                return instance;

            foreach (var argument in namedArguments)
            {
                var name = argument.NameEquals!.Name.Identifier.ValueText;
                var property = t.GetProperty(name, BindingFlags.Public);
                if (property == null)
                    continue;

                property.SetValue(instance, model.GetConstantValue(argument.Expression));
            }

            return instance;
        }

        public static bool InParent(this SyntaxNode obj, SyntaxNode? node)
        {
            var r = obj;
            while (r != null)
            {
                if (r == node)
                    return true;
                r = r.Parent;
            }

            return false;
        }
    }
}