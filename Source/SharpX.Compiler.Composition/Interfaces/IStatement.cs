using SharpX.Compiler.Composition.Abstractions;

namespace SharpX.Compiler.Composition.Interfaces
{
    /// <summary>
    ///     The name Statement is used for convenience, but it refers to the entire set of source code subparts (structures
    ///     that have no priority and whose order of addition is meaningful).
    /// </summary>
    public interface IStatement
    {
        public void WriteTo(SourceBuilder sb);
    }
}