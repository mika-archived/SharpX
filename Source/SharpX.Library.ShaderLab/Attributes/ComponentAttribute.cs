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
        private static readonly Regex HasGenericNameRegex = new("<&[A-Z][A-Za-z0-9]*>", RegexOptions.Compiled);

        public string? Name { get; }

        public ComponentAttribute(string? name = null)
        {
            Name = name;
        }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(true, nameof(Name))]
#else
#endif
        public bool IsValidName()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return false;
            return ValidComponentNameRegex.IsMatch(Name);
        }

        public bool HasGenericName()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return false;
            return HasGenericNameRegex.IsMatch(Name);
        }

        public string GetActualName(string generics)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return string.Empty;

            return HasGenericNameRegex.Replace(Name, $"<{generics}>");
        }
    }
}