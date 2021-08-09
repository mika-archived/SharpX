namespace SharpX.Compiler.Composition.Interfaces
{
    public interface ISourceContext
    {
        string ToSourceString();

        public T? OfType<T>() where T : ISourceContext
        {
            if (typeof(T) == GetType())
                return (T) this;
            return default;
        }
    }
}