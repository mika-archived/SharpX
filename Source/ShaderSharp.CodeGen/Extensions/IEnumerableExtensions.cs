using System.Collections.Generic;
using System.Linq;

namespace ShaderSharp.CodeGen.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T[]> Combination<T>(this IEnumerable<T> items, int pick, bool withRepetition)
        {
            if (pick == 1)
            {
                foreach (var item in items) yield return new[] { item };

                yield break;
            }

            foreach (var item in items)
            {
                var leftSide = new[] { item };
                var remaining = withRepetition ? items : items.SkipWhile(w => !w.Equals(item)).Skip(1).ToList();

                foreach (var rightSide in Combination(remaining, pick - 1, withRepetition)) yield return leftSide.Concat(rightSide).ToArray();
            }
        }
    }
}