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
                var s = Builtin.Max(Builtin.Max(Builtin.Distance((SlFloat2)a, (SlFloat2)b), Builtin.Distance((SlFloat2)b, (SlFloat2)c)), Builtin.Distance((SlFloat2)a, (SlFloat2)c));
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

        private SlFloat3 GetMovedVertexForVoxelization(SlFloat3 vertex, SlFloat3 normal)
        {
            var offset = Builtin.Mul<SlFloat3>(UnityCg.UnityObjectToWorld, new SlFloat4(Globals.VoxelOffsetX, Globals.VoxelOffsetY, Globals.VoxelOffsetZ, 0.0f)).XYZ;
            return new SlFloat3(
                                vertex.X + offset.X + normal.X * Globals.VoxelOffsetN,
                                vertex.Y + offset.Y + normal.Y * Globals.VoxelOffsetN,
                                vertex.Z + offset.Z + normal.Z * Globals.VoxelOffsetN
                               );
        }

        private SlFloat3 GetMovedVertexForHolograph(SlFloat3 vertex, SlFloat3 normal, SlFloat3 localPos)
        {
            var wave = Globals.Time.X - Builtin.Floor(Globals.Time.X) - (SlFloat) 0.5;
            var diff = -localPos.Y - wave;
            var height = 0 <= diff && diff <= 0.1 ? Builtin.Cos((diff - 0.05f) * 30) * Globals.TriangleHolographHeight : 0;
            var multiply = height < 0 ? 0 : height;

            return new SlFloat3(
                                vertex.X + normal.X * multiply,
                                vertex.Y +  normal.Y * multiply,
                                vertex.Z +  normal.Z * multiply
                               );
        }

        private Geometry2Fragment GetStreamDataForVoxelization(SlFloat3 vertex, SlFloat3 normal, SlFloat2 uv, SlFloat2 oNormal)
        {
#if SHADER_SHADOWCASTER
            var pos1 = GetMovedVertexForVoxelization(vertex, (SlFloat3) oNormal);
            var cos = Builtin.Dot(normal, Builtin.Normalize(UnityCg.UnityWorldSpaceLightDir(pos1)));
            var pos2 = pos1 - normal * UnityCg.UnityLightShadowBias.Z * Builtin.Sqrt(1 - cos * cos);

            return new() { Vertex = UnityCg.UnityApplyLinearShadowBias(UnityCg.UnityWorldToClipPos(new SlFloat4(pos2, 1.0f))) };
#else
            var moved = GetMovedVertexForVoxelization(vertex, (SlFloat3) oNormal);

            return new()
            {
                Vertex = UnityCg.UnityWorldToClipPos(moved),
                Normal = normal,
                TexCoord = uv,
                WorldPos = moved,
                LocalPos = Builtin.Mul<SlFloat3>(UnityCg.UnityWorldToObject, new SlFloat4(moved, 1.0f)),
            };
#endif
        }

        private Geometry2Fragment GetStreamDataForHolograph(SlFloat3 vertex, SlFloat3 normal, SlFloat2 uv, SlFloat3 localPos)
        {
#if SHADER_TRIANGLE_HOLOGRAPH
            var moved = GetMovedVertexForHolograph(vertex, normal, localPos);

            return new()
            {
                Vertex = UnityCg.UnityWorldToClipPos(moved),
                Normal = normal,
                TexCoord = uv,
                WorldPos = moved,
                LocalPos = Builtin.Mul<SlFloat3>(UnityCg.UnityWorldToObject, moved),
            };
#else
            return new() { };
#endif
        }

        private Geometry2Fragment GetStreamDataForNonGeometry(SlFloat3 vertex, SlFloat3 normal, SlFloat2 uv, SlFloat3 localPos)
        {
#if SHADER_SHADOWCASTER
            var pos1 = GetMovedVertexForVoxelization(vertex, (SlFloat3) oNormal);
            var cos = Builtin.Dot(normal, Builtin.Normalize(UnityCg.UnityWorldSpaceLightDir(pos1)));
            var pos2 = pos1 - normal * UnityCg.UnityLightShadowBias.Z * Builtin.Sqrt(1 - cos * cos);

            return new() { Vertex = UnityCg.UnityApplyLinearShadowBias(UnityCg.UnityWorldToClipPos(new SlFloat4(pos2, 1.0f))) };
#else
            return new()
            {
                Vertex = UnityCg.UnityWorldToClipPos(vertex),
                Normal = normal,
                TexCoord = uv,
                WorldPos = vertex,
                LocalPos = Builtin.Mul<SlFloat3>(UnityCg.UnityWorldToObject, new SlFloat4(vertex, 1.0f)),
            };
#endif
        }

        private Geometry2Fragment GetStreamDataForWireframe(SlFloat3 vertex, SlFloat3 normal, SlFloat3 bary)
        {
#if SHADER_WIREFRAME
            return new()
            {
                Vertex = UnityCg.UnityWorldToClipPos(vertex),
                Normal = normal,
                WorldPos = vertex,
                LocalPos = Builtin.Mul<SlFloat3>(UnityCg.UnityWorldToObject, new SlFloat4(vertex, 1.0f)),
                Bary = bary
            };
#else
            return new() { };
#endif
        }

        [Function("gs")]
        [GeometryMain]
        [MaxVertexCount(24)]
        public void GeometryMain([InputPrimitive(InputPrimitiveAttribute.InputPrimitives.Triangle)] Vertex2Geometry[] i, [Semantic("SV_PRIMITIVEID")] SlUint id, [InOut] ITriangleStream<Geometry2Fragment> stream)
        {
#if SHADER_WIREFRAME || SHADER_SHADOWCASTER
            if (!Globals.EnableVoxelization)
            {
                Compiler.AnnotatedStatement("unroll", () =>
                {
                    for (SlInt j = 0; j < 3; j++)
                    {
                        var vertex = i[j].WorldPos.XYZ;
                        var uv = i[j].TexCoord;
                        var normal = i[j].Normal;
                        var localPos = i[j].LocalPos;

                        stream.Append(GetStreamDataForNonGeometry(vertex, normal, uv, localPos));
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

            var o = CalcNormal((SlFloat3)p1, (SlFloat3)p2, (SlFloat3)p3);

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
            var f = Globals.EnableVoxelAnimation ? t / 250f * o * (d1 + d2 + d3) * signs[(int)Builtin.Round(r2)] : 0;

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

                stream.Append(GetStreamDataForVoxelization(a + f, n, uv, (SlFloat2) o));
                stream.Append(GetStreamDataForVoxelization(b + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(c + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(d + f, n, uv, (SlFloat2)o));
                stream.RestartStrip();
            }

            // bottom
            {
                var a = GetVertex(center, hx, -hy, hz);
                var b = GetVertex(center, -hx, -hy, hz);
                var c = GetVertex(center, hx, -hy, -hz);
                var d = GetVertex(center, -hz, -hy, -hz);

                var n = CalcNormal(a, b, c);

                stream.Append(GetStreamDataForVoxelization(a + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(b + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(c + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(d + f, n, uv, (SlFloat2)o));
                stream.RestartStrip();
            }

            // left side
            {
                var a = GetVertex(center, hx, hy, hz);
                var b = GetVertex(center, hx, -hy, hz);
                var c = GetVertex(center, hx, hy, -hz);
                var d = GetVertex(center, hz, -hy, -hz);

                var n = CalcNormal(a, b, c);

                stream.Append(GetStreamDataForVoxelization(a + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(b + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(c + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(d + f, n, uv, (SlFloat2)o));
                stream.RestartStrip();
            }

            // right side
            {
                var a = GetVertex(center, hx, hy, hz);
                var b = GetVertex(center, -hx, hy, hz);
                var c = GetVertex(center, hx, -hy, hz);
                var d = GetVertex(center, -hz, -hy, hz);

                var n = CalcNormal(a, b, c);

                stream.Append(GetStreamDataForVoxelization(a + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(b + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(c + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(d + f, n, uv, (SlFloat2)o));
                stream.RestartStrip();
            }

            // foreground
            {
                var a = GetVertex(center, hx, hy, hz);
                var b = GetVertex(center, -hx, hy, hz);
                var c = GetVertex(center, hx, -hy, hz);
                var d = GetVertex(center, -hz, -hy, hz);

                var n = CalcNormal(a, b, c);

                stream.Append(GetStreamDataForVoxelization(a + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(b + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(c + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(d + f, n, uv, (SlFloat2)o));
                stream.RestartStrip();
            }

            // background
            {
                var a = GetVertex(center, hx, hy, -hz);
                var b = GetVertex(center, hx, -hy, -hz);
                var c = GetVertex(center, -hx, hy, -hz);
                var d = GetVertex(center, -hz, -hy, -hz);

                var n = CalcNormal(a, b, c);

                stream.Append(GetStreamDataForVoxelization(a + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(b + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(c + f, n, uv, (SlFloat2)o));
                stream.Append(GetStreamDataForVoxelization(d + f, n, uv, (SlFloat2)o));
                stream.RestartStrip();
            }
#elif SHADER_TRIANGLE_HOLOGRAPH
            if (!Globals.EnableTriangleHolograph)
                return;

            var normal = (i[0].Normal + i[1].Normal + i[2].Normal) / 3;
            var localPos = (i[0].LocalPos + i[1].LocalPos + i[2].LocalPos) / 3;

            Compiler.AnnotatedStatement("unroll", () =>
            {
                for (SlInt j = 0; j < 3; j++)
                {
                    var vertex = i[j].Vertex.XYZ;
                    var uv = i[j].TexCoord;

                    stream.Append(GetStreamDataForHolograph(vertex, normal, uv, localPos));
                }
            });

            stream.RestartStrip();
#elif SHADER_WIREFRAME
            if (!Globals.EnableWireframe)
                return;

            var vert1 = i[0];
            var vert2 = i[1];
            var vert3 = i[2];

            stream.Append(GetStreamDataForWireframe(vert1.WorldPos.XYZ, vert1.Normal, new SlFloat3(1, 0, 0)));
            stream.Append(GetStreamDataForWireframe(vert2.WorldPos.XYZ, vert2.Normal, new SlFloat3(0, 1, 0)));
            stream.Append(GetStreamDataForWireframe(vert3.WorldPos.XYZ, vert3.Normal, new SlFloat3(0, 0, 1)));
            stream.RestartStrip();
#endif

        }
    }
}