using System;
using System.Collections.Immutable;

using SharpX.Library.ShaderLab.Interfaces;

#nullable enable

namespace SharpX.Library.ShaderLab.Abstractions
{
    public class ShaderPassDefinition : IShader
    {
        public string? AlphaToMask { get; protected set; }

        public string? Blend { get; protected set; }

        public (string, string?)? BlendOp { get; protected set; }

        public string? ColorMask { get; protected set; }

        public string? Cull { get; protected set; }

        public string? ZTest { get; protected set; }

        public string? ZWrite { get; protected set; }

        public (float, float)? Offset { get; protected set; }

        public ImmutableDictionary<string, string> Pragmas { get; }

        public ImmutableArray<Type> ShaderReferences { get; }

        public string? ShaderVariant { get; protected set; }

        public StencilDefinition? Stencil { get; protected set; }

        public ImmutableDictionary<string, string> Tags { get; protected set; } = ImmutableDictionary<string, string>.Empty;

        public string? Name { get; protected set; }

        public ShaderPassDefinition(ImmutableDictionary<string, string> pragmas, ImmutableArray<Type> shaders)
        {
            Pragmas = pragmas;
            ShaderReferences = shaders;
        }
    }
}