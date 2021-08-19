using System;
using System.Collections.Immutable;

using SharpX.Library.ShaderLab.Interfaces;

#nullable enable

namespace SharpX.Library.ShaderLab.Abstractions
{
    // Q. Why not interface?
    // A. Because Roslyn's Emit throws a CS0535 error.
    public abstract class ShaderLabDefinition : IShader
    {
        public string Name { get; }

        public Type Properties { get; }

        public ImmutableArray<SubShaderDefinition> SubShaders { get; }

        public string? Fallback { get; protected set; }

        public Type? CustomEditor { get; protected set; }

        public ShaderLabDefinition(string name, Type properties, ImmutableArray<SubShaderDefinition> subShaders)
        {
            Name = name;
            Properties = properties;
            SubShaders = subShaders;
        }
    }
}