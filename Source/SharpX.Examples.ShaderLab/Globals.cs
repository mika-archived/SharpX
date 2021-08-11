using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Examples.ShaderLab
{
    [Export("core.{extension}")]
    public static class Globals
    {
        [External]
        [GlobalMember]
        [Property("_Time")]
        public static SlFloat4 Time { get; }

        [GlobalMember]
        [Property("_Color")]
        public static SlFloat4 Color { get; }

        [GlobalMember]
        [Property("_Alpha")]
        public static SlFloat Alpha { get; }

        [GlobalMember]
        [Property("_MainTex")]
        public static Sampler2D MainTexture { get; }
    }
}