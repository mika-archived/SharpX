using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Library.ShaderLab.Predefined
{
    [Include("UnityCG.cginc")]
    [Component("appdata_base")]
    [External]
    public class AppDataBase
    {
        [Property("vertex")]
        public SlFloat4 Vertex { get; set; }

        [Property("normal")]
        public SlFloat3 Normal { get; set; }

        [Property("texcoord")]
        public SlFloat4 TexCoord { get; set; }
    }
}