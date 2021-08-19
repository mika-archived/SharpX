using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Enums;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Examples.ShaderLab.HLSL
{
    [Export("core.{extension}")]
    public static class Globals
    {
        #region Metadata

        [GlobalMember]
        [Property("_BlendMode")]
        [DisplayName("Blend Mode")]
        [Enum(typeof(BlendMode))]
        public static BlendMode BlendMode { get; } = BlendMode.Opaque;

        #endregion

        #region Unity Global Fields

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

        #endregion

        #region Main

        [GlobalMember]
        [Property("_MainTex")]
        [MainTexture]
        [DisplayName("Texture")]
        public static Sampler2D MainTexture { get; } = "white";

        [GlobalMember]
        [Property("_Color")]
        [DisplayName("Main Color")]
        [Color]
        public static SlFloat4 Color { get; } = new(0, 0, 0, 1);

        [GlobalMember]
        [Property("_Alpha")]
        [DisplayName("Alpha Transparent")]
        [Range(0.0f, 1.0f)]
        public static SlFloat Alpha { get; } = 0;

        #endregion

        #region Mesh Clipping

        [GlobalMember]
        [Property("_EnableMeshClipping")]
        [DisplayName("Enable Mesh Clipping")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        public static SlBool EnableMeshClipping { get; } = true;

        [GlobalMember]
        [Property("_MeshClippingMode")]
        [DisplayName("Mesh Clipping Mode")]
        [Enum(typeof(ClippingMode))]
        public static ClippingMode MeshClippingMode { get; } = ClippingMode.Left;

        [GlobalMember]
        [Property("_MeshClippingWidth")]
        [DisplayName("Mesh Clipping Width")]
        [Range(0.0f, 1.0f)]
        public static SlFloat MeshClippingWidth { get; } = 1;

        [GlobalMember]
        [Property("_MeshClippingOffset")]
        [DisplayName("Mesh Clipping Offset")]
        [Range(0.0f, 1.0f)]
        public static SlFloat MeshClippingOffset { get; } = 0;

        #endregion

        #region Triangle Holograph

        [GlobalMember]
        [Property("_EnableTriangleHolograph")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        public static SlBool EnableTriangleHolograph { get; } = false;

        [GlobalMember]
        [Property("_TriangleHolographHeight")]
        [DisplayName("Tris Holograph Height")]
        public static SlFloat TriangleHolographHeight { get; } = 0;

        [GlobalMember]
        [Property("_TriangleHolographAlpha")]
        [DisplayName("Tris Holograph Alpha")]
        [Range(0.0f, 1.0f)]
        public static SlFloat TriangleHolographAlpha { get; } = 1;

        #endregion

        #region Voxelization

        [GlobalMember]
        [Property("_EnableVoxelization")]
        [DisplayName("Enable Voxelization")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        public static SlBool EnableVoxelization { get; } = true;

        [GlobalMember]
        [Property("_VoxelSource")]
        [DisplayName("Voxel Source")]
        [Enum(typeof(VoxelSource))]
        public static VoxelSource VoxelSource { get; } = VoxelSource.ShaderProperty;

        [GlobalMember]
        [Property("_VoxelMinSize")]
        [DisplayName("Voxel Minimal Size")]
        public static SlFloat VoxelMinSize { get; } = 0;

        [GlobalMember]
        [Property("_VoxelSize")]
        [DisplayName("Voxel Size")]
        public static SlFloat VoxelSize { get; } = 0.0125f;

        [GlobalMember]
        [Property("_VoxelOffsetN")]
        [DisplayName("Voxel Offset Normal")]
        public static SlFloat VoxelOffsetN { get; } = 0;

        [GlobalMember]
        [Property("_VoxelOffsetX")]
        [DisplayName("Voxel Offset X")]
        public static SlFloat VoxelOffsetX { get; } = 0;

        [GlobalMember]
        [Property("_VoxelOffsetY")]
        [DisplayName("Voxel Offset Y")]
        public static SlFloat VoxelOffsetY { get; } = 0;

        [GlobalMember]
        [Property("_VoxelOffsetZ")]
        [DisplayName("Voxel Offset Z")]
        public static SlFloat VoxelOffsetZ { get; } = 0;

        [GlobalMember]
        [Property("_VoxelAnimation")]
        [DisplayName("Enable Voxel Animation")]
        public static SlBool EnableVoxelAnimation { get; } = true;

        [GlobalMember]
        [Property("_VoxelUVSamplingSource")]
        [Enum(typeof(UvSamplingSource))]
        public static UvSamplingSource VoxelUvSamplingSource { get; } = UvSamplingSource.Center;

        #endregion

        #region Thin Out

        [GlobalMember]
        [Property("_EnableThinOut")]
        [DisplayName("Enable ThinOut")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        public static SlBool EnableThinOut { get; } = false;

        [GlobalMember]
        [Property("_ThinOutSource")]
        [DisplayName("ThinOut Source")]
        [Enum(typeof(ThinOutSource))]
        public static ThinOutSource ThinOutSource { get; } = ThinOutSource.MaskTexture;

        [GlobalMember]
        [Property("_ThinOutMaskTex")]
        [DisplayName("ThinOut Mask Texture")]
        [NoScaleOffset]
        public static Sampler2D ThinOutMaskTexture { get; } = "white";

        [GlobalMember]
        [Property("_ThinOutNoiseTex")]
        [DisplayName("ThinOut Noise Texture")]
        [NoScaleOffset]
        public static Sampler2D ThinOutNoiseTexture { get; } = "white";

        [GlobalMember]
        [Property("_ThinOutNoiseThresholdR")]
        [DisplayName("ThinOut Noise Threshold R")]
        public static SlFloat ThinOutNoiseThresholdR { get; } = 1;

        [GlobalMember]
        [Property("_ThinOutNoiseThresholdG")]
        [DisplayName("ThinOut Noise Threshold G")]
        public static SlFloat ThinOutNoiseThresholdG { get; } = 1;

        [GlobalMember]
        [Property("_ThinOutNoiseThresholdB")]
        [DisplayName("ThinOut Noise Threshold B")]
        public static SlFloat ThinOutNoiseThresholdB { get; } = 1;

        [GlobalMember]
        [Property("_ThinOutMinSize")]
        [DisplayName("ThinOut Minimal Size")]
        public static SlFloat ThinOutMinSize { get; } = 1;

        #endregion

        #region Wireframe

        [GlobalMember]
        [Property("_EnableWireframe")]
        [DisplayName("Enable Wireframe")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        public static SlBool EnableWireframe { get; } = false;

        [GlobalMember]
        [Property("_WireframeColor")]
        [DisplayName("Wireframe Color")]
        [Color]
        public static SlFloat4 WireframeColor { get; } = new(0, 0, 0, 1);

        [GlobalMember]
        [Property("_WireframeThickness")]
        [DisplayName("Wireframe Thickness")]
        [Range(0, 1)]
        public static SlFloat WireframeThickness { get; } = 0.125f;

        #endregion

        #region Stencil

        [GlobalMember]
        [Property("_StencilRef")]
        [DisplayName("Stencil Ref")]
        public static SlInt StencilRef { get; } = 1;

        [GlobalMember]
        [Property("_StencilCompare")]
        [DisplayName("Stencil Compare")]
        [Enum("UnityEngine.Rendering.CompareFunction")]
        public static CompareFunction StencilCompare { get; } = CompareFunction.Always;

        [GlobalMember]
        [Property("_StencilKeep")]
        [DisplayName("Stencil Keep")]
        [Enum("UnityEngine.Rendering.StencilOp")]
        public static StencilOp StencilPass { get; } = StencilOp.Keep;

        #endregion

        #region Shader Properties

        [GlobalMember]
        [Property("_BlendSrcFactor")]
        [DisplayName("Blend Src Factor")]
        [HideInInspector]
        [Enum("UnityEngine.Rendering.BlendMode")]
        public static SlInt BlendSrcFactor { get; } = 5;

        [GlobalMember]
        [Property("_BlendDestFactor")]
        [DisplayName("Blend Dest Factor")]
        [HideInInspector]
        [Enum("UnityEngine.Rendering.BlendMode")]
        public static SlInt BlendDestFactor { get; } = 10;

        [GlobalMember]
        [Property("_Culling")]
        [DisplayName("Culling")]
        [Enum("UnityEngine.Rendering.CullMode")]
        public static Culling Culling { get; } = Culling.Off;

        [GlobalMember]
        [Property("_ZWrite")]
        [DisplayName("ZWrite")]
        [Enum("Off", 0, "On", 1)]
        public static Switch ZWrite { get; } = Switch.On;

        #endregion
    }
}