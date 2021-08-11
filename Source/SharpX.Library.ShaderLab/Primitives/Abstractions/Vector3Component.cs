using SharpX.CodeGen.ShaderLab.Attributes;

namespace SharpX.Library.ShaderLab.Primitives.Abstractions
{
    [Swizzle(4, "X", "Y", "Z")]
    [Swizzle(4, "R", "G", "B")]
    public partial class Vector3Component<T1, T2, T3, T4> { }
}