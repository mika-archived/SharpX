using System.Collections.Generic;
using System.Collections.Immutable;

using SharpX.Library.ShaderLab.Abstractions;

#nullable enable

namespace SharpX.Examples.ShaderLab.Shader
{
    internal class SakuraShaderLod0 : SubShaderDefinition
    {
        public SakuraShaderLod0() : base(ImmutableArray.Create<ShaderPassDefinition>(new VoxelGeometryPass(), new WireframePass(), new TriangleHolographPass(), new ShadowCasterPass()))
        {
            Tags = new Dictionary<string, string>
            {
                { "Queue", "Geometry" },
                { "RenderType", "Opaque" },
                { "IgnoreProjector", "False" },
                { "DisableBatching", "True" }
            }.ToImmutableDictionary();
        }
    }
}