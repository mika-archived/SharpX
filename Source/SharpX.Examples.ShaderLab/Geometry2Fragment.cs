using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;
using SharpX.Library.ShaderLab.Statements;

namespace SharpX.Examples.ShaderLab
{
    [Component("g2f")]
    [Export("core.{extension}")]
    public class Geometry2Fragment : Vertex2Geometry
    {
        [Semantic("TEXCOORD3")]
        [Property("bary")]
        public SlFloat3 Bary { get; set; }
    }
}