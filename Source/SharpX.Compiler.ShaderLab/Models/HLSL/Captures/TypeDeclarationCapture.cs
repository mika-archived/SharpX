using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Captures
{
    public class TypeDeclarationCapture
    {
        private readonly CapturedAs _captured;
        private readonly TypeInfo? _info;
        private readonly SemanticModel _model;
        private readonly ITypeSymbol? _symbol;

        private TypeDeclarationCapture(TypeInfo info, SemanticModel model, CapturedAs captured)
        {
            _info = info;
            _model = model;
            _captured = captured;
        }

        private TypeDeclarationCapture(ITypeSymbol symbol, SemanticModel model, CapturedAs captured)
        {
            _symbol = symbol;
            _model = model;
            _captured = captured;
        }

        public static TypeDeclarationCapture Capture(TypeSyntax node, SemanticModel model)
        {
            var info = model.GetTypeInfo(node);
            return new TypeDeclarationCapture(info, model, CapturedAs.Info);
        }

        public static TypeDeclarationCapture Capture(TypeInfo info, SemanticModel model)
        {
            return new(info, model, CapturedAs.Info);
        }

        public static TypeDeclarationCapture Capture(ITypeSymbol symbol, SemanticModel model)
        {
            return new(symbol, model, CapturedAs.Symbol);
        }

        public string GetActualName()
        {
            if (HasAttribute<ComponentAttribute>())
            {
                var attr = GetAttribute<ComponentAttribute>();
                if (attr != null && attr.IsValidName())
                    return attr.Name;
            }

            return _captured switch
            {
                CapturedAs.Info => _info!.Value.Type?.Name ?? "",
                CapturedAs.Symbol => _symbol!.Name,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return GetAttribute<T>() != null;
        }

        public T? GetAttribute<T>() where T : Attribute
        {
            return _captured switch
            {
                CapturedAs.Info => _info!.Value.Type.GetAttribute<T>(_model),
                CapturedAs.Symbol => _symbol.GetAttribute<T>(_model),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private enum CapturedAs
        {
            Info,

            Symbol
        }
    }
}