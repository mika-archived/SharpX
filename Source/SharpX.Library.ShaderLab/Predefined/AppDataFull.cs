using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Library.ShaderLab.Predefined
{
    [Include("UnityCG.cginc")]
    [Component("appdata_full")]
    [External]
    public class AppDataFull
    {
        [Property("vertex")]
        public SlFloat4 Vertex { get; set; }

        [Property("tangent")]
        public SlFloat4 Tangent { get; set; }

        [Property("normal")]
        public SlFloat3 Normal { get; set; }

        [Property("texcoord")]
        public SlFloat4 TexCoord { get; set; }

        [Property("texcoord1")]
        public SlFloat4 TexCoord1 { get; set; }

        [Property("texcoord2")]
        public SlFloat4 TexCoord2 { get; set; }

        [Property("texcoord3")]
        public SlFloat4 TexCoord3 { get; set; }

        [Property("color")]
        public SlFixed4 Color { get; set; }
    }
}