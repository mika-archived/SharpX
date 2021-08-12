using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Captures
{
    internal class PropertyDeclarationCapture
    {
        private readonly SemanticModel _model;
        private readonly PropertyDeclarationSyntax _node;

        public PropertyDeclarationCapture(PropertyDeclarationSyntax node, SemanticModel model)
        {
            _node = node;
            _model = model;
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return _node.HasAttribute<T>(_model);
        }

        public T? GetAttribute<T>() where T : Attribute
        {
            return _node.GetAttribute<T>(_model);
        }

        public string GetIdentifierName()
        {
            if (HasAttribute<PropertyAttribute>())
            {
                var attr = GetAttribute<PropertyAttribute>();
                if (attr?.IsValidName() == true)
                    return attr.Alternative;
            }

            return _node.Identifier.ValueText;
        }

        public string GetDeclaredType()
        {
            return TypeDeclarationCapture.Capture(_node.Type, _model).GetActualName();
        }
    }
}