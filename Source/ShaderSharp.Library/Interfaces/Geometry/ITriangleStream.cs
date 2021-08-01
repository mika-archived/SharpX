using ShaderSharp.Compiler.Abstractions.Attributes;

namespace ShaderSharp.Library.Interfaces.Geometry
{
    [Component("TriangleStream<&TOutput>")]
    [External]
    public interface ITriangleStream<in TOutput> : IStreamOutputObject<TOutput> where TOutput : IGeometryOutput { }
}