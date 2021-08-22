using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using SharpX.Examples.ShaderLab.HLSL;
using SharpX.Library.ShaderLab.Abstractions;

#nullable enable

namespace SharpX.Examples.ShaderLab.Shader
{
    internal class VoxelGeometryPass : ShaderPassDefinition
    {
        private static readonly ImmutableDictionary<string, string> ShaderPragmas = new Dictionary<string, string>
        {
            { "require", "geometry" },
            { "target", "4.5" },
            { "vertex", "vs" },
            { "geometry", "gs" },
            { "fragment", "fs" }
        }.ToImmutableDictionary();

        private static readonly ImmutableArray<Type> Shaders = ImmutableArray.Create(typeof(Vertex2Geometry), typeof(Geometry2Fragment), typeof(VertexShader), typeof(GeometryShader), typeof(FragmentShader));


        public VoxelGeometryPass() : base(ShaderPragmas, Shaders)
        {
            Name = "Avatars Voxel Geometry";
            ShaderVariant = "voxelization";
            Blend = "[_BlendSrcFactor] [_BlendDestFactor]";
            Cull = "[_Culling]";
            Stencil = new Stencil();
            ZWrite = "[_ZWrite]";
        }
    }
}