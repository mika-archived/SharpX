namespace SharpX.Compiler.ShaderLab.Models.Shader
{
    internal record ShaderProperty(string Type, string Name, string DisplayName, object? Default, string[] InspectorAttributes)
    {
        public string ConvertedType
        {
            get
            {
                switch (Type)
                {
                    case "SlInt":
                        return "Int";

                    case "SlFloat":
                        return "Float";

                    case "SlFloat4":
                        return "Color";

                    case "Sampler1D":
                        return "1D";

                    case "Sampler2D":
                        return "2D";

                    case "Sampler3D":
                        return "3D";

                    case "SamplerCUBE":
                        return "CUBE";

                    case { } when Type.StartsWith("Range"):
                        return Type;
                }

                return "Int";
            }
        }
    }
}