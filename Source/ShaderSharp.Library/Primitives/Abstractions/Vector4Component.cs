using ShaderSharp.Library.Attributes.Internal;
using ShaderSharp.Library.Primitives.Interfaces;

namespace ShaderSharp.Library.Primitives.Abstractions
{
    [Swizzle(4, "X", "Y", "Z", "W")]
    [Swizzle(4, "R", "G", "B", "A")]
    public abstract partial class Vector4Component<T> : IVectorComponent<T> { }
}