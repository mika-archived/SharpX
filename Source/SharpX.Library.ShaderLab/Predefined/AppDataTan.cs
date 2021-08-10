using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Library.ShaderLab.Predefined
{
    [Include("UnityCG.cginc")]
    [Component("appdata_tan")]
    [External]
    public class AppDataTan
    {
        [Property("vertex")]
        public SlFloat4 Vertex { get; set; }

        [Property("tangent")]
        public SlFloat4 Tangent { get; set; }

        [Property("normal")]
        public SlFloat3 Normal { get; set; }

        [Property("texcoord")]
        public SlFloat4 TexCoord { get; set; }
    }
}