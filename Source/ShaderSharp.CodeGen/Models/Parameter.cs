namespace ShaderSharp.CodeGen.Models
{
    public record Parameter
    {
        public string Name { get; init; }

        public string Component { get; set; }

        public string Type { get; set; }

        public string Element { get; set; }
    }
}