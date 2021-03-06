using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Declarators
{
    public class TypeDeclarationDeclarator
    {
        private readonly SemanticModel _model;
        private readonly TypeDeclarationSyntax _node;

        private TypeDeclarationDeclarator(TypeDeclarationSyntax node, SemanticModel model)
        {
            _node = node;
            _model = model;
        }

        public static TypeDeclarationDeclarator Create(TypeDeclarationSyntax node, SemanticModel model)
        {
            return node switch
            {
                ClassDeclarationSyntax c => new TypeDeclarationDeclarator(c, model),
                InterfaceDeclarationSyntax i => new TypeDeclarationDeclarator(i, model),
                RecordDeclarationSyntax r => new TypeDeclarationDeclarator(r, model),
                StructDeclarationSyntax s => new TypeDeclarationDeclarator(s, model),
                _ => throw new ArgumentOutOfRangeException(nameof(node))
            };
        }

        public bool IsNestedDeclaration()
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

        public bool IsInherited<T>()
        {
            return _node.HasInherit<T>(_model);
        }

        public string GetDeclarationName()
        {
            if (HasAttribute<ComponentAttribute>())
            {
                var attr = GetAttribute<ComponentAttribute>();
                if (attr?.IsValidName() == true)
                    return attr.Name!;
            }

            return _node.Identifier.ValueText;
        }
    }
}