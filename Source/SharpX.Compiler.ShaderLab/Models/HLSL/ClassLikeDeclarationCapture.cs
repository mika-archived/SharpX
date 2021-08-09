using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    internal class ClassLikeDeclarationCapture
    {
        private readonly CapturedType _capturedType;
        private readonly SemanticModel _model;
        private readonly CSharpSyntaxNode _node;

        private ClassLikeDeclarationCapture(CSharpSyntaxNode node, SemanticModel model, CapturedType type)
        {
            _node = node;
            _model = model;
            _capturedType = type;
        }

        public static ClassLikeDeclarationCapture? Capture(CSharpSyntaxNode node, SemanticModel model)
        {
            return node switch
            {
                ClassDeclarationSyntax c => new ClassLikeDeclarationCapture(c, model, CapturedType.Class),
                InterfaceDeclarationSyntax i => new ClassLikeDeclarationCapture(i, model, CapturedType.Interface),
                RecordDeclarationSyntax r => new ClassLikeDeclarationCapture(r, model, CapturedType.Record),
                StructDeclarationSyntax s => new ClassLikeDeclarationCapture(s, model, CapturedType.Struct),
                _ => null
            };
        }

        public bool IsNested()
        {
            var info = _model.GetSymbolInfo(_node);
            if (info.Symbol is not INamedTypeSymbol symbol)
                return false;
            return symbol.ToDisplayString().Contains("+");
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return _node.HasAttribute<T>(_model);
        }

        public T? GetAttribute<T>() where T : Attribute
        {
            return _node.GetAttribute<T>(_model);
        }

        public bool HasInherit<T>()
        {
            return _capturedType switch
            {
                CapturedType.Class => ((ClassDeclarationSyntax) _node).HasInherit<T>(_model),
                CapturedType.Interface => ((InterfaceDeclarationSyntax) _node).HasInherit<T>(_model),
                CapturedType.Record => ((RecordDeclarationSyntax) _node).HasInherit<T>(_model),
                CapturedType.Struct => ((StructDeclarationSyntax) _node).HasInherit<T>(_model),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string GetDeclarationName()
        {
            if (_node.HasAttribute<ComponentAttribute>(_model))
            {
                var attr = _node.GetAttribute<ComponentAttribute>(_model);
                if (!string.IsNullOrWhiteSpace(attr?.Name))
                    return attr.Name;
            }

            return _capturedType switch
            {
                CapturedType.Class => ((ClassDeclarationSyntax) _node).Identifier.ValueText,
                CapturedType.Interface => ((InterfaceDeclarationSyntax) _node).Identifier.ValueText,
                CapturedType.Record => ((RecordDeclarationSyntax) _node).Identifier.ValueText,
                CapturedType.Struct => ((StructDeclarationSyntax) _node).Identifier.ValueText,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private enum CapturedType
        {
            Class,

            Interface,

            Record,

            Struct
        }
    }
}