using System;
using System.Text.RegularExpressions;

namespace ShaderSharp.Compiler.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ComponentAttribute : Attribute
    {
        private static readonly Regex ValidComponentNameRegex = new("^[a-zA-Z][a-zA-Z0-9]+$", RegexOptions.Compiled);
        public string Name { get; }

        public ComponentAttribute(string name)
        {
            Name = name;
        }

        public bool IsValidName()
        {
            return ValidComponentNameRegex.IsMatch(Name);
        }
    }
}