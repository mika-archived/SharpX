namespace SharpX.Compiler.Composition.Interfaces
{
    public interface INestableStatement : IStatement
    {
        void AddSourcePart(INestableStatement statement);

        void AddSourcePart(IStatement statement);
    }
}