using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    internal class FieldLikeDeclarationCapture
    {
        private readonly CapturedType _capturedType;
        private readonly SemanticModel _model;
        private readonly CSharpSyntaxNode _node;

        private FieldLikeDeclarationCapture(CSharpSyntaxNode node, SemanticModel model, CapturedType type)
        {
            _node = node;
            _model = model;
            _capturedType = type;
        }

        public static FieldLikeDeclarationCapture? Capture(CSharpSyntaxNode node, SemanticModel model)
        {
            return node switch
            {
                FieldDeclarationSyntax f => new FieldLikeDeclarationCapture(f, model, CapturedType.Field),
                PropertyDeclarationSyntax p => new FieldLikeDeclarationCapture(p, model, CapturedType.Property),
                _ => null
            };
        }

        public bool IsStaticMember()
        {
            return _capturedType switch
            {
                CapturedType.Field => ((FieldDeclarationSyntax) _node).Modifiers.Any(SyntaxKind.StaticKeyword),
                CapturedType.Property => ((PropertyDeclarationSyntax) _node).Modifiers.Any(SyntaxKind.StaticKeyword),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string GetIdentifierName()
        {
            if (HasAttribute<PropertyAttribute>())
            {
                var attr = GetAttribute<PropertyAttribute>()!;
                if (attr.IsValidName())
                    return attr.Alternative;
            }

            return _capturedType switch
            {
                CapturedType.Field => ((FieldDeclarationSyntax) _node).Declaration.Variables.First().Identifier.ValueText,
                CapturedType.Property => ((PropertyDeclarationSyntax) _node).Identifier.ValueText,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public TypeDeclarationCapture GetDeclaredType()
        {
            return _capturedType switch
            {
                CapturedType.Field => TypeDeclarationCapture.Capture(((FieldDeclarationSyntax) _node).Declaration.Type, _model),
                CapturedType.Property => TypeDeclarationCapture.Capture(((PropertyDeclarationSyntax) _node).Type, _model),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return _node.HasAttribute<T>(_model);
        }

        public T? GetAttribute<T>() where T : Attribute
        {
            return _node.GetAttribute<T>(_model);
        }

        private enum CapturedType
        {
            Field,

            Property
        }
    }
}