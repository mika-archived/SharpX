using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.Composition.Interfaces
{
    public interface IStructuredComponent
    {
        public int Priority { get; set; }

        public string Name { get; }

        public void WriteTo(SourceBuilder sb);
    }
}