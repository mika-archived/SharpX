using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Library.ShaderLab.Predefined
{
    [External]
    public static class UnityInjection
    {
        [GlobalMember]
        [Property("UNITY_NEAR_CLIP_VALUE")]
        public static SlFloat NearClipValue { get; }

        [GlobalMember]
        [Property("UNITY_MATRIX_MVP")]
        public static SlFloat4x4 MatrixMVP { get; }

        [GlobalMember]
        [Property("UNITY_MATRIX_MV")]
        public static SlFloat4x4 MatrixMV { get; }

        [GlobalMember]
        [Property("UNITY_MATRIX_V")]
        public static SlFloat4x4 MatrixV { get; }

        [GlobalMember]
        [Property("UNITY_MATRIX_P")]
        public static SlFloat4x4 MatrixP { get; }

        [GlobalMember]
        [Property("UNITY_MATRIX_VP")]
        public static SlFloat4x4 MatrixVP { get; }

        [GlobalMember]
        [Property("UNITY_MATRIX_T_MV")]
        public static SlFloat4x4 MatrixTMV { get; }

        [GlobalMember]
        [Property("UNITY_MATRIX_IT_MV")]
        public static SlFloat4x4 MatrixITMV { get; }

        [GlobalMember]
        [Property("unity_ObjectToWorld")]
        public static SlFloat4x4 ObjectToWorld { get; }

        [GlobalMember]
        [Property("unity_WorldToObject")]
        public static SlFloat4x4 WorldToObject { get; }

        [GlobalMember]
        [Property("_WorldSpaceCameraPos")]
        public static SlFloat3 WorldSpaceCameraPos { get; }

        [GlobalMember]
        [Property("_ProjectionParams")]
        public static SlFloat4 ProjectionParams { get; }

        [GlobalMember]
        [Property("_ScreenParams")]
        public static SlFloat4 ScreenParams { get; }

        [GlobalMember]
        [Property("_ZBufferParams")]
        public static SlFloat4 ZBufferParams { get; }

        [GlobalMember]
        [Property("unity_OrthoParams")]
        public static SlFloat4 OrthoParams { get; }

        [GlobalMember]
        [Property("unity_CameraProjection")]
        public static SlFloat4x4 CameraProjection { get; }

        [GlobalMember]
        [Property("unity_CameraInvProjection")]
        public static SlFloat4x4 CameraInvProjection { get; }

        [GlobalMember]
        [Property("unity_CameraWorldClipPlanes")]
        public static SlFloat4 CameraWorldClipPlanes { get; }

        [GlobalMember]
        [Property("_Time")]
        public static SlFloat4 Time { get; }

        [GlobalMember]
        [Property("_SinTime")]
        public static SlFloat4 SinTime { get; }

        [GlobalMember]
        [Property("_CosTime")]
        public static SlFloat4 CosTime { get; }

        [GlobalMember]
        [Property("unity_DeltaTime")]
        public static SlFloat4 Delta { get; }
    }
}