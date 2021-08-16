using SharpX.Examples.ShaderLab.Stubs;
using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Functions;
using SharpX.Library.ShaderLab.Interfaces;
using SharpX.Library.ShaderLab.Primitives;
using SharpX.Library.ShaderLab.Statements;

namespace SharpX.Examples.ShaderLab
{
    [Export("geom.{extension}")]
    public class GeometryShader
    {
        private SlFloat2 GetUVFromOptions(SlFloat2 a, SlFloat2 b, SlFloat2 c)
        {
            switch (Globals.VoxelUvSamplingSource)
            {
                case UvSamplingSource.Center:
                    return (a + b + c) / 3;

                case UvSamplingSource.First:
                    return a;

                case UvSamplingSource.Second:
                    return b;

                case UvSamplingSource.Last:
                    return c;

                default:
                    return new SlFloat2(0);
            }
        }

        private SlFloat GetMaxDistanceFor(SlFloat a, SlFloat b, SlFloat c)
        {
            if (Globals.VoxelSource == VoxelSource.Vertex)
            {
                var s = Builtin.Max(Builtin.Max(Builtin.Distance((SlFloat2) a, (SlFloat2) b), Builtin.Distance((SlFloat2) b, (SlFloat2) c)), Builtin.Distance((SlFloat2) a, (SlFloat2) c));
                return s < Globals.VoxelMinSize ? Globals.VoxelMinSize : s;
            }

            return Globals.VoxelSize;
        }

        private SlFloat GetRandom(SlFloat2 st, SlInt seed)
        {
            return Builtin.Frac(Builtin.Sin(Builtin.Dot(st.XY, new SlFloat2(12.9898f, 78.233f)) + seed) * 43758.5453123f);
        }

        private SlFloat3 CalcNormal(SlFloat3 a, SlFloat3 b, SlFloat3 c)
        {
            return Builtin.Normalize(Builtin.Cross(b - a, c - a));
        }

        private SlFloat3 GetVertex(SlFloat3 center, SlFloat x, SlFloat y, SlFloat z)
        {
            return center + Builtin.Mul<SlFloat3>(UnityCg.UnityObjectToWorld, new SlFloat4(x, y, z, 0.0f)).XYZ;
        }

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

            var uv = GetUVFromOptions(i[0].TexCoord, i[1].TexCoord, i[2].TexCoord);

            if (Globals.EnableThinOut)
            {
                if (Globals.ThinOutSource == ThinOutSource.MaskTexture)
                {
                    var m = Builtin.Tex2Dlod(Globals.ThinOutMaskTexture, new SlFloat4(uv, 0.0f, 0.0f));
                    if (m.R <= 0.5)
                        return;
                }

                if (Globals.ThinOutSource == ThinOutSource.NoiseTexture)
                {
                    var n = Builtin.Tex2Dlod(Globals.ThinOutNoiseTexture, new SlFloat4(uv, 0.0f, 0.0f));
                    if (n.R < Globals.ThinOutNoiseThresholdR && n.G < Globals.ThinOutNoiseThresholdG && n.B < Globals.ThinOutNoiseThresholdB)
                        return;
                }
            }

            var p1 = i[0].Vertex;
            var p2 = i[1].Vertex;
            var p3 = i[2].Vertex;

            var center = ((p1 + p2 + p3) / 3).XYZ;

            var x = GetMaxDistanceFor(p1.X, p2.X, p3.X);
            var y = GetMaxDistanceFor(p1.Y, p2.Y, p3.Y);
            var z = GetMaxDistanceFor(p1.Z, p2.Z, p3.Z);

            if (Globals.EnableThinOut && Globals.ThinOutSource == ThinOutSource.ShaderProperty)
                if (x + y + z <= Globals.ThinOutMinSize)
                    return;

            var r1 = GetRandom(i[0].TexCoord, id);
            var r2 = GetRandom(i[1].TexCoord, id);
            var r3 = GetRandom(i[2].TexCoord, id);

            var o = CalcNormal((SlFloat3) p1, (SlFloat3) p2, (SlFloat3) p3);

            SlFloat3[] dirs =
            {
                new(1.0f, 0.0f, 0.0f),
                new(0.0f, 1.0f, 0.0f),
                new(0.0f, 0.0f, 1.0f)
            };

            SlFloat[] signs = { new(1.0f), new(-1.0f) };

            var d1 = dirs[0] * r1;
            var d2 = dirs[1] * r2;
            var d3 = dirs[2] * r3;

            var t = Globals.SinTime.W * r1 + Globals.CosTime.W * r3;
            var f = Globals.EnableVoxelAnimation ? t / 250f * o * (d1 + d2 + d3) * signs[(int) Builtin.Round(r2)] : 0;

            SlFloat s = Globals.VoxelSource == VoxelSource.ShaderProperty ? 0 : 0;

            var hx = x / 2 + s;
            var hy = y / 2 + s;
            var hz = z / 2 + s;

            // top
            {
                var a = GetVertex(center, hx, hy, hz);
                var b = GetVertex(center, hx, hy, -hz);
                var c = GetVertex(center, -hx, hy, hz);
                var d = GetVertex(center, -hz, hy, -hz);

                var n = CalcNormal(a, b, c);
            }
        }
    }
}