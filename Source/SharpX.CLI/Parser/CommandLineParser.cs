using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SharpX.CLI.Parser.Attributes;

namespace SharpX.CLI.Parser
{
    internal static class CommandLineParser
    {
        public static T Parse<T>(string[] arguments)
        {
            var dict = new Dictionary<string, string>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var availableKeys = ParseObjectKeys(properties);
            var isKey = true;
            var currentKey = "";

            foreach (var argument in arguments)
                if (isKey)
                {
                    var normalizedKey = argument.StartsWith("--") ? argument.Substring(2) : argument.Substring(1);

                    if (!availableKeys.ContainsKey(normalizedKey)) 
                        continue;

                    var targetProperty = availableKeys[normalizedKey];
                    var property = properties.First(w => w.Name == targetProperty);
                    if (property.PropertyType == typeof(bool))
                    {
                        dict.Add(property.Name, "true");
                    }
                    else
                    {
                        isKey = false;
                        currentKey = property.Name;
                    }
                }
                else
                {
                    dict.Add(currentKey, argument);
                    isKey = true;
                }

            ValidateRequiredKeys(properties, dict);

            var t = Activator.CreateInstance<T>();
            foreach (var pair in dict)
            {
                var property = properties.First(w => w.Name == pair.Key);
                if (property.PropertyType == typeof(bool))
                    property.SetMethod!.Invoke(t, new object?[] { bool.Parse(pair.Value) });
                if (property.PropertyType == typeof(string))
                    property.SetMethod!.Invoke(t, new object?[] { pair.Value });
            }

            return t;
        }

        private static Dictionary<string, string> ParseObjectKeys(IEnumerable<PropertyInfo> properties)
        {
            var dict = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                dict.Add(property.Name, property.Name);

                var option = property.GetCustomAttribute<OptionAttribute>();
                if (!string.IsNullOrWhiteSpace(option?.Name))
                    dict.Add(option.Name, property.Name);
            }

            return dict;
        }


        private static void ValidateRequiredKeys(IEnumerable<PropertyInfo> properties, Dictionary<string, string> args)
        {
            foreach (var property in properties)
            {
                var required = property.GetCustomAttribute<RequiredAttribute>();
                if (required == null)
                    continue;

                if (!args.ContainsKey(property.Name))
                    throw new InvalidOperationException($"required option `{property.Name}` is unspecified");
            }
        }
    }
}