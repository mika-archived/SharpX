using ShaderSharp.Compiler.Abstractions.Attributes;

namespace ShaderSharp.Library.Interfaces.Geometry
{
    [Component("PointStream<&TOutput>")]
    [External]
    public interface IPointStream<in TOutput> : IStreamOutputObject<TOutput> where TOutput : IGeometryOutput { }
}