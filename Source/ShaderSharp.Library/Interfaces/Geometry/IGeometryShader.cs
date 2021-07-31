using ShaderSharp.Library.Interfaces.Geometry;

namespace ShaderSharp.Library.Interfaces
{
    public interface IGeometryShader<out TGlobals, in TInput, TOutput, in TStreamOutputObject> where TGlobals : IGlobals where TInput : IGeometryInput where TOutput : IGeometryOutput where TStreamOutputObject : IStreamOutputObject<TOutput>
    {
        protected TGlobals Globals { get; }

        void GeometryMain(TInput[] i, TStreamOutputObject o);
    }
}