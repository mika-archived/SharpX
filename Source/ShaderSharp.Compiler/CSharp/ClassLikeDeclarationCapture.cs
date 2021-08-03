using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShaderSharp.Compiler.CSharp
{
    public class ClassLikeDeclarationCapture
    {
        private readonly CapturedType _capturedType;
        private readonly SyntaxNode _node;
        private readonly SemanticModel _semanticModel;

        private ClassLikeDeclarationCapture(SyntaxNode node, SemanticModel semanticModel, CapturedType capturedType)
        {
            _node = node;
            _semanticModel = semanticModel;
            _capturedType = capturedType;
        }

        public static ClassLikeDeclarationCapture Capture(ClassDeclarationSyntax declaration, SemanticModel semanticModel)
        {
            return new(declaration, semanticModel, CapturedType.Class);
        }

        public static ClassLikeDeclarationCapture Capture(StructDeclarationSyntax declaration, SemanticModel semanticModel)
        {
            return new(declaration, semanticModel, CapturedType.Struct);
        }

        public static ClassLikeDeclarationCapture Capture(InterfaceDeclarationSyntax declaration, SemanticModel semanticModel)
        {
            return new(declaration, semanticModel, CapturedType.Interface);
        }

        public static ClassLikeDeclarationCapture Capture(RecordDeclarationSyntax declaration, SemanticModel semanticModel)
        {
            return new(declaration, semanticModel, CapturedType.Record);
        }

        public string GetDeclarationName()
        {
            return _capturedType switch
            {
                CapturedType.Class => ((ClassDeclarationSyntax) _node).Identifier.ValueText,
                CapturedType.Struct => ((StructDeclarationSyntax) _node).Identifier.ValueText,
                CapturedType.Interface => ((InterfaceDeclarationSyntax) _node).Identifier.ValueText,
                CapturedType.Record => ((RecordDeclarationSyntax) _node).Identifier.ValueText,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool HasAttribute<T>()
        {
            return GetAttributeData<T>() != null;
        }

        public AttributeData GetAttributeData<T>()
        {
            var rawSymbol = _semanticModel.GetDeclaredSymbol(_node);
            if (rawSymbol is not INamedTypeSymbol symbol)
                return null;

            var fullyQualifiedMetadataName = typeof(T).FullName;
            if (string.IsNullOrWhiteSpace(fullyQualifiedMetadataName))
                return null;

            var t = _semanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
            return symbol.GetAttributes().FirstOrDefault(w => w.AttributeClass != null && w.AttributeClass.Equals(t, SymbolEqualityComparer.Default));
        }

        public bool IsNested()
        {
            var info = _semanticModel.GetSymbolInfo(_node);
            if (info.Symbol is not INamedTypeSymbol symbol)
                return false;
            return symbol.ToDisplayString().Contains("+");
        }

        private enum CapturedType
        {
            Class,

            Struct,

            Interface,

            Record
        }
    }
}