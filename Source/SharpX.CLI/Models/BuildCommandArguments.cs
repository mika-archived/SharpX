using SharpX.CLI.Parser.Attributes;

namespace SharpX.CLI.Models
{
    internal class BuildCommandArguments
    {
        [Option("project")]
        public string Project { get; set; }

        [Option("solution")]
        public string Solution { get; set; }
    }
}