﻿using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Functions;
using SharpX.Library.ShaderLab.Predefined;
using SharpX.Library.ShaderLab.Primitives;

namespace SharpX.Examples.ShaderLab
{
    [Export("vert.{extension}")]
    public class VertexShader
    {
        public SlInt HelloWorld()
        {
            return 1;
        }

        [Function("vs")]
        [VertexMain]
        public Vertex2Geometry VertexMain(AppDataFull v)
        {
            var o = new Vertex2Geometry
            {
                Vertex = UnityCg.UnityObjectToClipPos(v.Vertex),
                Normal = UnityCg.UnityObjectToWorldNormal(v.Normal),
                TexCoord = UnityCg.TransformTexture(v.TexCoord, Globals.MainTexture),
                WorldPos = Builtin.Mul<SlFloat3>(UnityCg.UnityObjectToWorld, v.Vertex),
                LocalPos = v.Vertex.XYZ
            };

            return o;
        }
    }
}