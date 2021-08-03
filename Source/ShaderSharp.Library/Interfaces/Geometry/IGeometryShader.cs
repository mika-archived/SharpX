namespace ShaderSharp.Library.Interfaces.Geometry
{
    public interface IGeometryShader<in TInput, TOutput, in TStreamOutputObject> where TInput : IGeometryInput where TOutput : IGeometryOutput where TStreamOutputObject : IStreamOutputObject<TOutput>
    {
        void GeometryMain(TInput[] i, TStreamOutputObject o);
    }
}