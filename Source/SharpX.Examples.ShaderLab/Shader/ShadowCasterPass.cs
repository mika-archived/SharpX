using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using SharpX.Examples.ShaderLab.HLSL;
using SharpX.Library.ShaderLab.Abstractions;

#nullable enable

namespace SharpX.Examples.ShaderLab.Shader
{
    internal class ShadowCasterPass : ShaderPassDefinition
    {
        private static readonly ImmutableDictionary<string, string> ShaderPragmas = new Dictionary<string, string>
        {
            { "require", "geometry" },
            { "target", "4.5" },
            { "vertex", Configuration.GetShaderEntryPoint(typeof(VertexShader), Configuration.EntryPoint.VertexShader) },
            { "geometry", Configuration.GetShaderEntryPoint(typeof(GeometryShader), Configuration.EntryPoint.GeometryShader) },
            { "fragment", Configuration.GetShaderEntryPoint(typeof(FragmentShader), Configuration.EntryPoint.FragmentShader) },
            { "multi_compile_shadowcaster", "" },
            { "multi_compile_fog", "" }
        }.ToImmutableDictionary();

        private static readonly ImmutableArray<Type> Shaders = ImmutableArray.Create(typeof(Vertex2Geometry), typeof(Geometry2Fragment), typeof(VertexShader), typeof(GeometryShader), typeof(FragmentShader));

        public ShadowCasterPass() : base(ShaderPragmas, Shaders)
        {
            ShaderVariant = "shadow-caster";
            Tags = new Dictionary<string, string>
            {
                { "LightMode", "ShadowCaster" }
            }.ToImmutableDictionary();
        }
    }
}