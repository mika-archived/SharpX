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
        [DefaultValue(BlendMode.Opaque)]
        public static BlendMode BlendMode { get; }

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
        [DefaultValue(@"""white"" { }")]
        public static Sampler2D MainTexture { get; }

        [GlobalMember]
        [Property("_Color")]
        [DisplayName("Main Color")]
        [Color]
        [DefaultValue("(0, 0, 0, 1)")]
        public static SlFloat4 Color { get; }

        [GlobalMember]
        [Property("_Alpha")]
        [DisplayName("Alpha Transparent")]
        [Range(0.0f, 1.0f)]
        [DefaultValue(0)]
        public static SlFloat Alpha { get; }

        #endregion

        #region Mesh Clipping

        [GlobalMember]
        [Property("_EnableMeshClipping")]
        [DisplayName("Enable Mesh Clipping")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        [DefaultValue(true)]
        public static SlBool EnableMeshClipping { get; }

        [GlobalMember]
        [Property("_MeshClippingMode")]
        [DisplayName("Mesh Clipping Mode")]
        [Enum(typeof(ClippingMode))]
        [DefaultValue(ClippingMode.Left)]
        public static ClippingMode MeshClippingMode { get; }

        [GlobalMember]
        [Property("_MeshClippingWidth")]
        [DisplayName("Mesh Clipping Width")]
        [Range(0.0f, 1.0f)]
        [DefaultValue(1.0f)]
        public static SlFloat MeshClippingWidth { get; }

        [GlobalMember]
        [Property("_MeshClippingOffset")]
        [DisplayName("Mesh Clipping Offset")]
        [Range(0.0f, 1.0f)]
        [DefaultValue(0)]
        public static SlFloat MeshClippingOffset { get; }

        #endregion

        #region Triangle Holograph

        [GlobalMember]
        [Property("_EnableTriangleHolograph")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        [DefaultValue(false)]
        public static SlBool EnableTriangleHolograph { get; }

        [GlobalMember]
        [Property("_TriangleHolographHeight")]
        [DisplayName("Tris Holograph Height")]
        [DefaultValue(0)]
        public static SlFloat TriangleHolographHeight { get; }

        [GlobalMember]
        [Property("_TriangleHolographAlpha")]
        [DisplayName("Tris Holograph Alpha")]
        [Range(0.0f, 1.0f)]
        [DefaultValue(1)]
        public static SlFloat TriangleHolographAlpha { get; }

        #endregion

        #region Voxelization

        [GlobalMember]
        [Property("_EnableVoxelization")]
        [DisplayName("Enable Voxelization")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        [DefaultValue(true)]
        public static SlBool EnableVoxelization { get; } 

        [GlobalMember]
        [Property("_VoxelSource")]
        [DisplayName("Voxel Source")]
        [Enum(typeof(VoxelSource))]
        [DefaultValue(VoxelSource.ShaderProperty)]
        public static VoxelSource VoxelSource { get; }

        [GlobalMember]
        [Property("_VoxelMinSize")]
        [DisplayName("Voxel Minimal Size")]
        [DefaultValue(0)]
        public static SlFloat VoxelMinSize { get; }

        [GlobalMember]
        [Property("_VoxelSize")]
        [DisplayName("Voxel Size")]
        [DefaultValue(0.0125f)]
        public static SlFloat VoxelSize { get; }

        [GlobalMember]
        [Property("_VoxelOffsetN")]
        [DisplayName("Voxel Offset Normal")]
        [DefaultValue(0)]
        public static SlFloat VoxelOffsetN { get; }

        [GlobalMember]
        [Property("_VoxelOffsetX")]
        [DisplayName("Voxel Offset X")]
        [DefaultValue(0)]
        public static SlFloat VoxelOffsetX { get; }

        [GlobalMember]
        [Property("_VoxelOffsetY")]
        [DisplayName("Voxel Offset Y")]
        [DefaultValue(0)]
        public static SlFloat VoxelOffsetY { get; }

        [GlobalMember]
        [Property("_VoxelOffsetZ")]
        [DisplayName("Voxel Offset Z")]
        [DefaultValue(0)]
        public static SlFloat VoxelOffsetZ { get; }

        [GlobalMember]
        [Property("_VoxelAnimation")]
        [DisplayName("Enable Voxel Animation")]
        [DefaultValue(true)]
        public static SlBool EnableVoxelAnimation { get; }

        [GlobalMember]
        [Property("_VoxelUVSamplingSource")]
        [Enum(typeof(UvSamplingSource))]
        [DefaultValue(UvSamplingSource.Center)]
        public static UvSamplingSource VoxelUvSamplingSource { get; }

        #endregion

        #region Thin Out

        [GlobalMember]
        [Property("_EnableThinOut")]
        [DisplayName("Enable ThinOut")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        [DefaultValue(false)]
        public static SlBool EnableThinOut { get; }

        [GlobalMember]
        [Property("_ThinOutSource")]
        [DisplayName("ThinOut Source")]
        [Enum(typeof(ThinOutSource))]
        [DefaultValue(ThinOutSource.MaskTexture)]
        public static ThinOutSource ThinOutSource { get; }

        [GlobalMember]
        [Property("_ThinOutMaskTex")]
        [DisplayName("ThinOut Mask Texture")]
        [NoScaleOffset]
        [DefaultValue(@"""white"" { }")]
        public static Sampler2D ThinOutMaskTexture { get; }

        [GlobalMember]
        [Property("_ThinOutNoiseTex")]
        [DisplayName("ThinOut Noise Texture")]
        [NoScaleOffset]
        [DefaultValue(@"""white"" { }")]
        public static Sampler2D ThinOutNoiseTexture { get; }

        [GlobalMember]
        [Property("_ThinOutNoiseThresholdR")]
        [DisplayName("ThinOut Noise Threshold R")]
        [DefaultValue(1)]
        public static SlFloat ThinOutNoiseThresholdR { get; }

        [GlobalMember]
        [Property("_ThinOutNoiseThresholdG")]
        [DisplayName("ThinOut Noise Threshold G")]
        [DefaultValue(1)]
        public static SlFloat ThinOutNoiseThresholdG { get; }

        [GlobalMember]
        [Property("_ThinOutNoiseThresholdB")]
        [DisplayName("ThinOut Noise Threshold B")]
        [DefaultValue(1)]
        public static SlFloat ThinOutNoiseThresholdB { get; }

        [GlobalMember]
        [Property("_ThinOutMinSize")]
        [DisplayName("ThinOut Minimal Size")]
        [DefaultValue(1)]
        public static SlFloat ThinOutMinSize { get; }

        #endregion

        #region Wireframe

        [GlobalMember]
        [Property("_EnableWireframe")]
        [DisplayName("Enable Wireframe")]
        [CustomInspectorAttribute("SSToggleWithoutKeyword")]
        [DefaultValue(false)]
        public static SlBool EnableWireframe { get; }

        [GlobalMember]
        [Property("_WireframeColor")]
        [DisplayName("Wireframe Color")]
        [Color]
        [DefaultValue("(0, 0, 0, 1)")]
        public static SlFloat4 WireframeColor { get; }

        [GlobalMember]
        [Property("_WireframeThickness")]
        [DisplayName("Wireframe Thickness")]
        [Range(0, 1)]
        [DefaultValue(0.125f)]
        public static SlFloat WireframeThickness { get; }

        #endregion

        #region Stencil

        [GlobalMember]
        [Property("_StencilRef")]
        [DisplayName("Stencil Ref")]
        [DefaultValue(1)]
        public static SlInt StencilRef { get; }

        [GlobalMember]
        [Property("_StencilCompare")]
        [DisplayName("Stencil Compare")]
        [Enum("UnityEngine.Rendering.CompareFunction")]
        [DefaultValue(CompareFunction.Always)]
        public static CompareFunction StencilCompare { get; }

        [GlobalMember]
        [Property("_StencilKeep")]
        [DisplayName("Stencil Keep")]
        [Enum("UnityEngine.Rendering.StencilOp")]
        [DefaultValue(StencilOp.Keep)]
        public static StencilOp StencilPass { get; }

        #endregion

        #region Shader Properties

        [GlobalMember]
        [Property("_BlendSrcFactor")]
        [DisplayName("Blend Src Factor")]
        [HideInInspector]
        [Enum("UnityEngine.Rendering.BlendMode")]
        [DefaultValue(5)]
        public static SlInt BlendSrcFactor { get; }

        [GlobalMember]
        [Property("_BlendDestFactor")]
        [DisplayName("Blend Dest Factor")]
        [HideInInspector]
        [Enum("UnityEngine.Rendering.BlendMode")]
        [DefaultValue(10)]
        public static SlInt BlendDestFactor { get; }

        [GlobalMember]
        [Property("_Culling")]
        [DisplayName("Culling")]
        [Enum("UnityEngine.Rendering.CullMode")]
        [DefaultValue(Culling.Off)]
        public static Culling Culling { get; }

        [GlobalMember]
        [Property("_ZWrite")]
        [DisplayName("ZWrite")]
        [Enum("Off", 0, "On", 1)]
        [DefaultValue(1)]
        public static Switch ZWrite { get; }

        #endregion
    }
}