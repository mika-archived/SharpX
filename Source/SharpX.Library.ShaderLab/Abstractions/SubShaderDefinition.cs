using System.Collections.Immutable;

using SharpX.Library.ShaderLab.Interfaces;

#nullable enable

namespace SharpX.Library.ShaderLab.Abstractions
{
    public class SubShaderDefinition : IShader
    {
        public IImmutableDictionary<string, string> Tags { get; protected set; } = ImmutableDictionary<string, string>.Empty;

        public uint? Lod { get; protected set; }

        public string? GrabPass { get; }

        public ImmutableArray<ShaderPassDefinition> Pass { get; }

        protected SubShaderDefinition(string pass)
        {
            GrabPass = pass;
            Pass = ImmutableArray<ShaderPassDefinition>.Empty;
        }

        protected SubShaderDefinition(ImmutableArray<ShaderPassDefinition> pass)
        {
            GrabPass = null;
            Pass = pass;
        }
    }
}