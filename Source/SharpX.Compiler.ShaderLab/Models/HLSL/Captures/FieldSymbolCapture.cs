using System;

using Microsoft.CodeAnalysis;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Captures
{
    internal class FieldSymbolCapture
    {
        private readonly SemanticModel _model;
        private readonly IFieldSymbol _symbol;

        public FieldSymbolCapture(IFieldSymbol symbol, SemanticModel model)
        {
            _symbol = symbol;
            _model = model;
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return GetAttribute<T>() != null;
        }

        public T? GetAttribute<T>() where T : Attribute
        {
            return _symbol.GetAttribute<T>(_model);
        }

        public string GetIdentifierName()
        {
            if (HasAttribute<PropertyAttribute>())
            {
                var attr = GetAttribute<PropertyAttribute>();
                if (attr?.IsValidName() == true)
                    return attr.Alternative;
            }

            return _symbol.Name;
        }

        public bool HasValidType()
        {
            return TypeDeclarationCapture.Capture(_symbol.Type, _model).HasValidType();
        }

        public string GetDeclaredType()
        {
            return TypeDeclarationCapture.Capture(_symbol.Type, _model).GetActualName();
        }

        public string? GetSemanticsName()
        {
            var attr = GetAttribute<SemanticAttribute>();
            if (attr == null || !attr.IsValidSemantics())
                return null;
            return attr.Semantic;
        }
    }
}