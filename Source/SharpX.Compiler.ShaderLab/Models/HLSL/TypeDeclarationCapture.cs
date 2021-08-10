using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL
{
    public class TypeDeclarationCapture
    {
        private readonly SemanticModel _model;
        private readonly TypeSyntax _node;

        private TypeDeclarationCapture(TypeSyntax node, SemanticModel model)
        {
            _node = node;
            _model = model;
        }

        public static TypeDeclarationCapture Capture(TypeSyntax node, SemanticModel model)
        {
            return new(node, model);
        }

        public string GetActualName()
        {
            if (HasAttribute<ComponentAttribute>())
            {
                var attr = GetAttribute<ComponentAttribute>();
                if (attr != null && attr.IsValidName())
                    return attr.Name;
            }

            return _node.ToString();
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return GetAttribute<T>() != null;
        }

        public T? GetAttribute<T>() where T : Attribute
        {
            var s = _model.GetTypeInfo(_node);
            return s.Type.GetAttribute<T>(_model);
        }
    }
}