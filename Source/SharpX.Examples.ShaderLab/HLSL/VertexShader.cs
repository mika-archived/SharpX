using SharpX.Examples.ShaderLab.Stubs;
using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Functions;
using SharpX.Library.ShaderLab.Predefined;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Examples.ShaderLab.HLSL
{
    [Export("vert.{extension}")]
    public class VertexShader
    {
        [Function("vs")]
        [VertexShader]
        public Vertex2Geometry VertexMain(AppDataFull v)
        {
            return new()
            {
                Vertex = UnityCg.UnityObjectToClipPos(v.Vertex),
                Normal = UnityCg.UnityObjectToWorldNormal(v.Normal),
                TexCoord = UnityCg.TransformTexture(v.TexCoord, Globals.MainTexture),
                WorldPos = Builtin.Mul<SlFloat3>(UnityCg.UnityObjectToWorld, v.Vertex),
                LocalPos = v.Vertex.XYZ
            };
        }
    }
}