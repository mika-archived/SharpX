using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

#nullable enable

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ComponentAttribute : Attribute
    {
        private static readonly Regex ValidComponentNameRegex = new("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        public string? Name { get; }

        public ComponentAttribute(string? name = null)
        {
            Name = name;
        }

        [MemberNotNullWhen(true, nameof(Name))]
        public bool IsValidName()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return false;
            return ValidComponentNameRegex.IsMatch(Name);
        }
    }
}