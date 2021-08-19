using System.Collections.Immutable;

using SharpX.Examples.ShaderLab.HLSL;
using SharpX.Library.ShaderLab.Abstractions;
using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Interfaces;

#nullable enable

namespace SharpX.Examples.ShaderLab.Shader
{
    [Export("Avatars.{extension}")]
    internal class SakuraShader : ShaderLabDefinition
    {
        private static readonly ImmutableArray<SubShaderDefinition> Shaders = ImmutableArray.Create<SubShaderDefinition>(new SakuraShaderLod0());

        public SakuraShader() : base("NatsunekoLaboratory/Sakura Shader/Avatars", typeof(Globals), Shaders) { }
    }
}