﻿using ShaderSharp.Library.Attributes.Internal;
using ShaderSharp.Library.Primitives.Interfaces;

namespace ShaderSharp.Library.Primitives.Abstractions
{
    [Swizzle(4, "X", "Y")]
    [Swizzle(4, "R", "G")]
    public abstract partial class Vector2Component<T> : IVectorComponent<T> { }
}