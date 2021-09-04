using SharpX.CLI.Parser.Attributes;

namespace SharpX.CLI.Models
{
    internal class InitCommandArguments
    {
        [Option("path")]
        public string? Path { get; set; }
    }
}