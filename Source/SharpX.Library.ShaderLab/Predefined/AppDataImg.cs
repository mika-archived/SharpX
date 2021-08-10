using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Library.ShaderLab.Predefined
{
    [Include("UnityCG.cginc")]
    [Component("appdata_img")]
    [External]
    public class AppDataImg
    {
        [Property("vertex")]
        public SlFloat4 Vertex { get; set; }

        [Property("texcoord")]
        public SlHalf2 TexCoord { get; set; }
    }
}