using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using SharpX.Examples.ShaderLab.HLSL;
using SharpX.Library.ShaderLab.Abstractions;
using SharpX.Library.ShaderLab.Enums;

#nullable enable

namespace SharpX.Examples.ShaderLab.Shader
{
    internal class WireframePass : ShaderPassDefinition
    {
        private static readonly ImmutableDictionary<string, string> ShaderPragmas = new Dictionary<string, string>
        {
            { "require", "geometry" },
            { "target", "4.5" },
            { "vertex", "vs" },
            { "geometry", "gs" },
            { "fragment", "fs" }
        }.ToImmutableDictionary();

        private static readonly ImmutableArray<Type> Shaders = ImmutableArray.Create(typeof(VertexShader), typeof(GeometryShader), typeof(FragmentShader));

        public WireframePass() : base(ShaderPragmas, Shaders)
        {
            Name = "Avatars Wireframe";
            ShaderVariant = "SHADER_WIREFRAME";
            Blend = "SrcAlpha OneMinusSrcAlpha";
            Cull = Culling.Off.ToString();
            Stencil = new Stencil();
            ZWrite = "[_ZWrite]";
        }
    }
}