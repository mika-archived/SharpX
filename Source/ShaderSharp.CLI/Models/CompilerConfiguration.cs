namespace ShaderSharp.CLI.Models
{
    public record CompilerConfiguration
    {
        public string[] Sources { get; set; }

        public string[] References { get; set; }

        public string Out { get; set; }
    }
}