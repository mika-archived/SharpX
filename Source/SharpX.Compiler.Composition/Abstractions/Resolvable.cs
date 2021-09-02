namespace SharpX.Compiler.Composition.Abstractions
{
    public class Resolvable<T>
    {
        public T? Value { get; set; }

        public bool IsResolved => Value != null;
    }
}