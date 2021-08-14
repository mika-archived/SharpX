using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

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
            return Globals.WireframeColor;
        }
    }
}