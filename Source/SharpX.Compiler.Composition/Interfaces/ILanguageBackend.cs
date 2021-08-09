namespace SharpX.Compiler.Composition.Interfaces
{
    public interface ILanguageBackend
    {
        string Identifier { get; }

        void Initialize(ILanguageBackendContext context);
    }
}