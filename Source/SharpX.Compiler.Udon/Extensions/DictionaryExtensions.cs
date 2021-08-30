using System.Collections.Generic;

namespace SharpX.Compiler.Udon.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddIfValid<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (key != null)
                dict.Add(key, value);
        }
    }
}