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

        [External]
        [GlobalMember]
        [Property("_SinTime")]
        public static SlFloat4 SinTime { get; }

        [External]
        [GlobalMember]
        [Property("_CosTime")]
        public static SlFloat4 CosTime { get; }

        [GlobalMember]
        [Property("_MainTex")]
        public static Sampler2D MainTexture { get; }

        [GlobalMember]
        [Property("_Color")]
        public static SlFloat4 Color { get; }

        [GlobalMember]
        [Property("_Alpha")]
        public static SlFloat Alpha { get; }

        [GlobalMember]
        [Property("_BlendMode")]
        public static BlendMode BlendMode { get; }

        [GlobalMember]
        [Property("_EnableMeshClipping")]
        public static SlBool EnableMeshClipping { get; }

        [GlobalMember]
        [Property("_MeshClippingMode")]
        public static ClippingMode MeshClippingMode { get; }

        [GlobalMember]
        [Property("_MeshClippingWidth")]
        public static SlFloat MeshClippingWidth { get; }

        [GlobalMember]
        [Property("_MeshClippingOffset")]
        public static SlFloat MeshClippingOffset { get; }

        [GlobalMember]
        [Property("_EnableTriangleHolograph")]
        public static SlBool EnableTriangleHolograph { get; }

        [GlobalMember]
        [Property("_TriangleHolographHeight")]
        public static SlFloat TriangleHolographHeight { get; }

        [GlobalMember]
        [Property("_TriangleHolographAlpha")]
        public static SlFloat TriangleHolographAlpha { get; }

        [GlobalMember]
        [Property("_EnableVoxelization")]
        public static SlBool EnableVoxelization { get; }

        [GlobalMember]
        [Property("_VoxelSource")]
        public static VoxelSource VoxelSource { get; }

        [GlobalMember]
        [Property("_VoxelMinSize")]
        public static SlFloat VoxelMinSize { get; }

        [GlobalMember]
        [Property("_VoxelSize")]
        public static SlFloat VoxelSize { get; }

        [GlobalMember]
        [Property("_VoxelOffsetN")]
        public static SlFloat VoxelOffsetN { get; }

        [GlobalMember]
        [Property("_VoxelOffsetX")]
        public static SlFloat VoxelOffsetX { get; }

        [GlobalMember]
        [Property("_VoxelOffsetY")]
        public static SlFloat VoxelOffsetY { get; }

        [GlobalMember]
        [Property("_VoxelOffsetZ")]
        public static SlFloat VoxelOffsetZ { get; }

        [GlobalMember]
        [Property("_VoxelAnimation")]
        public static SlBool EnableVoxelAnimation { get; }

        [GlobalMember]
        [Property("_VoxelUVSamplingSource")]
        public static UvSamplingSource VoxelUvSamplingSource { get; }

        [GlobalMember]
        [Property("_EnableThinOut")]
        public static SlInt EnableThinOut { get; }

        [GlobalMember]
        [Property("_ThinOutSource")]
        public static ThinOutSource ThinOutSource { get; }

        [GlobalMember]
        [Property("_ThinOutMaskTex")]
        public static Sampler2D ThinOutMaskTexture { get; }

        [GlobalMember]
        [Property("_ThinOutNoiseTex")]
        public static Sampler2D ThinOutNoiseTexture { get; }

        [GlobalMember]
        [Property("_ThinOutNoiseThresholdR")]
        public static SlFloat ThinOutNoiseThresholdR { get; }

        [GlobalMember]
        [Property("_ThinOutNoiseThresholdG")]
        public static SlFloat ThinOutNoiseThresholdG { get; }

        [GlobalMember]
        [Property("_ThinOutNoiseThresholdB")]
        public static SlFloat ThinOutNoiseThresholdB { get; }

        [GlobalMember]
        [Property("_ThinOutMinSize")]
        public static SlFloat ThinOutMinSize { get; }

        [GlobalMember]
        [Property("_EnableWireframe")]
        public static SlBool EnableWireframe { get; }

        [GlobalMember]
        [Property("_WireframeColor")]
        public static SlFloat4 WireframeColor { get; }

        [GlobalMember]
        [Property("_WireframeThickness")]
        public static SlFloat WireframeThickness { get; }
    }
}