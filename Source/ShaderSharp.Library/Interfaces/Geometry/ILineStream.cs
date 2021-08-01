using ShaderSharp.Compiler.Abstractions.Attributes;

namespace ShaderSharp.Library.Interfaces.Geometry
{
    [Component("LineStream<&TOutput>")]
    [External]
    public interface ILineStream<in TOutput> : IStreamOutputObject<TOutput> where TOutput : IGeometryOutput { }
}