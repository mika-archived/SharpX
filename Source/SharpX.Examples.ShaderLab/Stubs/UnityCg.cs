using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Examples.ShaderLab.Stubs
{
    [Include("UnityCG.cginc")]
    [External]
    public static class UnityCg
    {
        [Property("unity_ObjectToWorld")]
        public static object UnityObjectToWorld { get; }

        [External]
        public static extern SlFloat4 UnityObjectToClipPos(SlFloat4 a);

        [External]
        public static extern SlFloat3 UnityObjectToWorldNormal(SlFloat3 a);

        [Function("TRANSFORM_TEX")]
        public static extern SlFloat2 TransformTexture(SlFloat4 a, Sampler2D b);
    }
}