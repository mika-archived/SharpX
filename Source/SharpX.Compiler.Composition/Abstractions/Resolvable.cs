using System.Diagnostics.CodeAnalysis;

namespace SharpX.Compiler.Composition.Abstractions
{
    public class Resolvable<T>
    {
        public T? ResolvedValue { get; private set; }

        [MemberNotNullWhen(true, nameof(ResolvedValue))]
        public bool IsResolved => ResolvedValue != null;

        public void Resolve(T value)
        {
            ResolvedValue = value;
        }

        public void UnResolve()
        {
            ResolvedValue = default;
        }
    }
}