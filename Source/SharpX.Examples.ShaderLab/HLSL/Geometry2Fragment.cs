using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Examples.ShaderLab.HLSL
{
    [Component("g2f")]
    [Export("core")]
    public class Geometry2Fragment : Vertex2Geometry
    {
        [Semantic("TEXCOORD3")]
        [Property("bary")]
        public SlFloat3 Bary { get; set; }
    }
}