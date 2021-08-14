using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;
using SharpX.Compiler.ShaderLab.Models.HLSL.Captures;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Declarators
{
    internal class MethodDeclarationDeclarator
    {
        private readonly SemanticModel _model;
        private readonly MethodDeclarationSyntax _node;

        public MethodDeclarationDeclarator(MethodDeclarationSyntax node, SemanticModel model)
        {
            _node = node;
            _model = model;
        }

        public bool HasAttribute<T>(string? target = null) where T : Attribute
        {
            if (string.IsNullOrWhiteSpace(target))
                return _node.HasAttribute<T>(_model);
            return GetAttribute<T>(target) != null;
        }

        public T? GetAttribute<T>(string? target = null) where T : Attribute
        {
            if (string.IsNullOrWhiteSpace(target))
                return _node.GetAttribute<T>(_model);

            foreach (var attributes in _node.AttributeLists)
            {
                if (attributes.Target?.Identifier.ValueText != target)
                    continue;

                return attributes.Attributes.Select(w => w.AsAttributeInstance<T>(_model)).FirstOrDefault(w => w != null);
            }

            return null;
        }

        public string GetIdentifierName()
        {
            if (HasAttribute<FunctionAttribute>())
            {
                var attr = GetAttribute<FunctionAttribute>();
                if (attr?.IsValidName() == true)
                    return attr.Alternative;
            }

            return _node.Identifier.ValueText;
        }

        public string GetDeclaredReturnType()
        {
            return TypeDeclarationCapture.Capture(_node.ReturnType, _model).GetActualName();
        }
    }
}