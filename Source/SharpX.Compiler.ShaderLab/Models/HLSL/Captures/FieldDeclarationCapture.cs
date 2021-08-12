using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Captures
{
    internal class FieldDeclarationCapture
    {
        private readonly SemanticModel _model;
        private readonly FieldDeclarationSyntax _node;

        public FieldDeclarationCapture(FieldDeclarationSyntax node, SemanticModel model)
        {
            _node = node;
            _model = model;
        }

        public string GetIdentifierName()
        {
            if (HasAttribute<PropertyAttribute>())
            {
                var attr = GetAttribute<PropertyAttribute>()!;
                if (attr.IsValidName())
                    return attr.Alternative;
            }

            return _node.Declaration.Variables.First().Identifier.ValueText;
        }

        public string GetDeclaredType()
        {
            return TypeDeclarationCapture.Capture(_node.Declaration.Type, _model).GetActualName();
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return _node.HasAttribute<T>(_model);
        }

        public T? GetAttribute<T>() where T : Attribute
        {
            return _node.GetAttribute<T>(_model);
        }
    }
}