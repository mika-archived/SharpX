using System.Collections.Immutable;

namespace SharpX.Compiler.CodeCleanup.Models
{
    internal class AllowList
    {
        public ImmutableArray<string> Attributes { get; set; }

        public ImmutableArray<string> Methods { get; set; }

        public ImmutableArray<string> Properties { get; set; }

        public ImmutableArray<string> Fields { get; set; }

        public ImmutableArray<string> Variables { get; set; }
    }
}