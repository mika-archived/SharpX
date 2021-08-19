using SharpX.Library.CodeCleanup.Attributes;
using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Examples.ShaderLab.Stubs
{
    [Include("UnityCG.cginc")]
    [External]
    [Stub]
    public static class UnityCg
    {
        [Property("unity_ObjectToWorld")]
        public static object UnityObjectToWorld { get; }

        [Property("unity_WorldToObject")]
        public static object UnityWorldToObject { get; }

        [Property("unity_LightShadowBias")]
        public static SlFloat4 UnityLightShadowBias { get; }

        [External]
        public static extern SlFloat4 UnityObjectToClipPos([Mark] SlFloat4 a);

        [External]
        public static extern SlFloat4 UnityWorldToClipPos([Mark] SlFloat3 a);

        [External]
        public static extern SlFloat4 UnityWorldToClipPos([Mark] SlFloat4 a);

        [External]
        public static extern SlFloat3 UnityObjectToWorldNormal([Mark] SlFloat3 a);

        [External]
        public static extern SlFloat3 UnityWorldSpaceLightDir([Mark] SlFloat3 a);

        [External]
        public static extern SlFloat4 UnityApplyLinearShadowBias([Mark] SlFloat4 a);

        [Function("TRANSFORM_TEX")]
        public static extern SlFloat2 TransformTexture([Mark] SlFloat4 a, [Mark] Sampler2D b);
    }
}