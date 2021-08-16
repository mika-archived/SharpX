using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;
using SharpX.Library.ShaderLab.Statements;

namespace SharpX.Examples.ShaderLab
{
    [Export("frag.{extension}")]
    public class FragmentShader
    {
        [FragmentMain]
        [Function("fs")]
        [return: Semantic("SV_TARGET")]
        public SlFloat4 Fragment(Geometry2Fragment i)
        {
#if SHADER_SHADOWCASTER
            Compiler.Raw("SHADOW_CASTER_FRAGMENT(i)");
#else
            return Globals.WireframeColor;
#endif
        }
    }
}