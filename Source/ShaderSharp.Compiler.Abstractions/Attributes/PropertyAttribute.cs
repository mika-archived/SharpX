using System;
using System.Text.RegularExpressions;

namespace ShaderSharp.Compiler.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        private static readonly Regex ValidComponentNameRegex = new("^[a-zA-Z][a-zA-Z0-9]+$", RegexOptions.Compiled);

        public string Name { get; }

        public PropertyAttribute(string name)
        {
            Name = name;
        }

        public bool IsValidName()
        {
            return ValidComponentNameRegex.IsMatch(Name);
        }
    }
}