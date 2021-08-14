namespace SharpX.Examples.ShaderLab
{
    public enum BlendMode
    {
        Opaque,

        Transparent
    }

    public enum ClippingMode
    {
        Left,

        Center,

        Right,

        Top,

        Bottom
    }

    public enum IndexSource
    {
        TexCoordZ,

        TexCoordW
    }

    public enum ThinOutSource
    {
        MaskTexture,

        NoiseTexture,

        ShaderProperty
    }

    public enum VoxelSource
    {
        Vertex,

        ShaderProperty
    }

    public enum UvSamplingSource
    {
        Center,

        First,

        Second,

        Last
    }
}