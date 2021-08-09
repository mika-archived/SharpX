using System.Collections.Generic;
using System.Linq;

namespace SharpX.CodeGen.ShaderLab.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class EnumerableExtensions
    {
        public static IEnumerable<T[]> Combination<T>(this IEnumerable<T> items, int pick, bool withRepetition)
        {
            if (pick == 1)
            {
                foreach (var item in items) yield return new[] { item };

                yield break;
            }

            var array = items.ToList();
            foreach (var item in array)
            {
                var leftSide = new[] { item };
                var remaining = withRepetition ? array : array.SkipWhile(w => !w!.Equals(item)).Skip(1).ToList();

                foreach (var rightSide in Combination(remaining, pick - 1, withRepetition)) yield return leftSide.Concat(rightSide).ToArray();
            }
        }
    }
}