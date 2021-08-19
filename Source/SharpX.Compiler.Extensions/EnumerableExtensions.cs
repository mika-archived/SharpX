using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpX.Compiler.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> obj, Action<T> action)
        {
            foreach (var item in obj)
                action.Invoke(item);
        }

        public static bool None<T>(this IEnumerable<T> obj, Func<T, bool> predicate)
        {
            return !obj.Any(predicate);
        }
    }
}