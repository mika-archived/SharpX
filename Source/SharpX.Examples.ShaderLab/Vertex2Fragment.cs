using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Examples.ShaderLab
{
    [Component("v2f")]
    [Export("core.{extension}")]
    public class Vertex2Fragment
    {
        [Semantic("SV_POSITION")]
        [Property("vertex")]
        public SlFloat4 Vertex { get; set; }

        [Semantic("NORMAL")]
        [Property("normal")]
        public SlFloat3 Normal { get; set; }

        [Semantic("TEXCOORD0")]
        [Property("texCoord")]
        public SlFloat2 TexCoord { get; set; }

        [Semantic("TEXCOORD1")]
        [Property("worldPos")]
        public SlFloat3 WorldPos { get; set; }

        [Semantic("TEXCOORD2")]
        [Property("localPos")]
        public SlFloat3 LocalPos { get; set; }
    }
}