using System.Diagnostics.CodeAnalysis;

namespace SharpX.Compiler.Composition.Abstractions
{
    public class Resolvable<T>
    {
        public T? ResolvedValue { get; set; }

        [MemberNotNullWhen(true, nameof(ResolvedValue))]
        public bool IsResolved => ResolvedValue != null;
    }
}