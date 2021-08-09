using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Models.Plugin
{
    internal class SourceContextGeneratorArgs : ISourceContextGeneratorArgs
    {
        private readonly string _extension;

        public SourceContextGeneratorArgs(string name, string extension)
        {
            Name = name;
            _extension = extension;
        }

        public string FullName => $"{Name}.{_extension}";

        public string Name { get; }
    }
}