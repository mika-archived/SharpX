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

        [Property("unity_WorldToObject")]
        public static object UnityWorldToObject {  get;}

        [Property("unity_LightShadowBias")]
        public static SlFloat4 UnityLightShadowBias { get; }

        [External]
        public static extern SlFloat4 UnityObjectToClipPos(SlFloat4 a);

        [External]
        public static extern SlFloat4 UnityWorldToClipPos(SlFloat3 a);

        [External]
        public static extern SlFloat4 UnityWorldToClipPos(SlFloat4 a);

        [External]
        public static extern SlFloat3 UnityObjectToWorldNormal(SlFloat3 a);

        [External]
        public static extern SlFloat3 UnityWorldSpaceLightDir(SlFloat3 a);

        [External]
        public static extern SlFloat4 UnityApplyLinearShadowBias(SlFloat4 a);

        [Function("TRANSFORM_TEX")]
        public static extern SlFloat2 TransformTexture(SlFloat4 a, Sampler2D b);
    }
}