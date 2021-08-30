using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

namespace SharpX.Compiler.Udon.Models
{
    // ReSharper disable once InconsistentNaming
    internal class ITypeSymbolEqualityComparer : IEqualityComparer<ITypeSymbol>
    {
        public bool Equals(ITypeSymbol? x, ITypeSymbol? y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Equals(y, SymbolEqualityComparer.Default);
        }

        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "<Pending>")]
        public int GetHashCode(ITypeSymbol obj)
        {
            return obj.GetHashCode();
        }
    }
}