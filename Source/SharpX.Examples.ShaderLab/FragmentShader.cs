using System;

using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Functions;
using SharpX.Library.ShaderLab.Primitives;
using SharpX.Library.ShaderLab.Statements;

namespace SharpX.Examples.ShaderLab
{
    [Export("frag.{extension}")]
    public class FragmentShader
    {
        [FragmentShader]
        [Function("fs")]
        [return: Semantic("SV_TARGET")]
        public SlFloat4 Fragment(Geometry2Fragment i)
        {
#if SHADER_SHADOWCASTER
            Compiler.Raw("SHADOW_CASTER_FRAGMENT(i)");
#elif SHADER_WIREFRAME
            var deltas = Builtin.Fwidth(i.Bary) * 1 * Globals.WireframeThickness;
            var baries = Builtin.Smoothstep(deltas,   deltas * (SlFloat)2, i.Bary);
            var bary = Builtin.Min(baries.X, Builtin.Min(baries.Y, baries.Z));

            Builtin.Clip(bary > 0.5 ? -1 : 0);

            return Globals.WireframeColor;
#elif SHADER_TRIANGLE_HOLOGRAPH
            return new SlFloat4(Builtin.Tex2D(Globals.MainTexture, i.TexCoord).RGB, Globals.TriangleHolographAlpha);
#else
            return new SlFloat4(Builtin.Tex2D(Globals.MainTexture, i.TexCoord).RGB, Globals.BlendMode == 0 ? 1 : Globals.Alpha);
#endif
        }
    }
}