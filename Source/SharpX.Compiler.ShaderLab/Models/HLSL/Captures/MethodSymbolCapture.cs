using System;

using Microsoft.CodeAnalysis;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Captures
{
    internal class MethodSymbolCapture
    {
        private readonly SemanticModel _model;
        private readonly IMethodSymbol _symbol;

        public MethodSymbolCapture(IMethodSymbol symbol, SemanticModel model)
        {
            _symbol = symbol;
            _model = model;
        }

        public bool IsStatic()
        {
            return _symbol.IsStatic;
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
            if (HasAttribute<FunctionAttribute>())
            {
                var attr = GetAttribute<FunctionAttribute>();
                if (attr?.IsValidName() == true)
                    return attr.Alternative;
            }

            return _symbol.Name;
        }
    }
}