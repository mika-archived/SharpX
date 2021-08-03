using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using ShaderSharp.Compiler.Abstractions.Attributes;
using ShaderSharp.Compiler.Extensions;

namespace ShaderSharp.Compiler.CSharp
{
    public class FieldLikeDeclarationCapture
    {
        private readonly CapturedType _capturedType;
        private readonly SyntaxNode _node;
        private readonly SemanticModel _semanticModel;

        private FieldLikeDeclarationCapture(SyntaxNode node, SemanticModel semanticModel, CapturedType capturedType)
        {
            _node = node;
            _semanticModel = semanticModel;
            _capturedType = capturedType;
        }

        public static FieldLikeDeclarationCapture Capture(FieldDeclarationSyntax declaration, SemanticModel semanticModel)
        {
            return new(declaration, semanticModel, CapturedType.Field);
        }

        public static FieldLikeDeclarationCapture Capture(PropertyDeclarationSyntax declaration, SemanticModel semanticModel)
        {
            return new(declaration, semanticModel, CapturedType.Property);
        }

        public bool IsStaticMember()
        {
            return _capturedType switch
            {
                CapturedType.Property => ((PropertyDeclarationSyntax) _node).Modifiers.Any(SyntaxKind.StaticKeyword),
                CapturedType.Field => ((FieldDeclarationSyntax) _node).Modifiers.Any(SyntaxKind.StaticKeyword),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string GetName()
        {
            var attr = GetAttributeDataOfDeclaration<PropertyAttribute>()?.AsAttributeInstance<PropertyAttribute>();
            if (attr == null || !attr.IsValidName())
                return _capturedType switch
                {
                    CapturedType.Property => ((PropertyDeclarationSyntax) _node).Identifier.ValueText,
                    CapturedType.Field => ((FieldDeclarationSyntax) _node).Declaration.Variables.FirstOrDefault()?.Identifier.ValueText,
                    _ => throw new ArgumentOutOfRangeException()
                };

            return attr.Name;
        }

        public string GetDeclarationType()
        {
            var attr = GetAttributeDataOfType<ComponentAttribute>()?.AsAttributeInstance<ComponentAttribute>();
            if (attr == null || !attr.IsValidName())
                return GetTypeSymbol().Name;
            return attr.Name;
        }

        public bool HasAttributeOfType<T>()
        {
            return GetAttributeDataOfType<T>() != null;
        }

        public AttributeData GetAttributeDataOfType<T>()
        {
            var symbol = GetTypeSymbol();
            var fullyQualifiedMetadataName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(fullyQualifiedMetadataName))
                return null;

            var t = _semanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
            return symbol.GetAttributes().FirstOrDefault(w => w.AttributeClass != null && w.AttributeClass.Equals(t, SymbolEqualityComparer.Default));
        }

        public bool HasAttributeOfDeclaration<T>()
        {
            return GetAttributeDataOfDeclaration<T>() != null;
        }

        public AttributeData GetAttributeDataOfDeclaration<T>()
        {
            var symbol = _semanticModel.GetDeclaredSymbol(_node);
            var fullyQualifiedMetadataName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(fullyQualifiedMetadataName))
                return null;

            var t = _semanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
            var attributes = symbol switch
            {
                IPropertySymbol s => s.GetAttributes(),
                IFieldSymbol s => s.GetAttributes(),
                _ => throw new ArgumentOutOfRangeException()
            };

            return attributes.FirstOrDefault(w => w.AttributeClass != null && w.AttributeClass.Equals(t, SymbolEqualityComparer.Default));
        }

        private ITypeSymbol GetTypeSymbol()
        {
            return _semanticModel.GetDeclaredSymbol(_node) switch
            {
                IPropertySymbol s => s.Type,
                IFieldSymbol s => s.Type,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private enum CapturedType
        {
            Property,

            Field
        }
    }
}