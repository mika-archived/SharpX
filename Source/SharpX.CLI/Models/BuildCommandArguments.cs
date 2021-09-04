using SharpX.CLI.Parser.Attributes;

namespace SharpX.CLI.Models
{
    internal class BuildCommandArguments
    {
        [Required]
        [Option("project")]
        public string Project { get; set; }
    }
}