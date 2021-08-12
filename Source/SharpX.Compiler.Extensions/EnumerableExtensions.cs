using System;
using System.Collections.Generic;

namespace SharpX.Compiler.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> obj, Action<T> action)
        {
            foreach (var item in obj)
                action.Invoke(item);
        }
    }
}