using SharpX.Examples.ShaderLab.Stubs;
using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Interfaces;
using SharpX.Library.ShaderLab.Primitives;
using SharpX.Library.ShaderLab.Statements;

namespace SharpX.Examples.ShaderLab
{
    [Export("geom.{extension}")]
    public class GeometryShader
    {
        [Function("gs")]
        [GeometryMain]
        [MaxVertexCount(24)]
        public void GeometryMain([InputPrimitive(InputPrimitiveAttribute.InputPrimitives.Triangle)]
                                 Vertex2Geometry[] i, [Semantic("SV_PRIMITIVEID")] SlUint id, [InOut] ITriangleStream<Geometry2Fragment> stream)
        {
            if (!Globals.EnableVoxelization)
            {
                ForStatement.AttributedFor("unroll", () =>
                {
                    for (var j = 0; j < 3; j++)
                    {
                        var vertex = i[j].WorldPos.XYZ;
                        var uv = i[j].TexCoord;
                        var normal = i[j].Normal;
                        var localPos = i[j].LocalPos;

                        stream.Append(new Geometry2Fragment
                        {
                            Vertex = UnityCg.UnityWorldToClipPos(vertex),
                            Normal = normal,
                            TexCoord = uv,
                            WorldPos = vertex,
                            LocalPos = localPos
                        });
                    }

                    stream.RestartStrip();
                });

                return;
            }

            stream.RestartStrip();
        }
    }
}