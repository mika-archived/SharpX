using System;
using System.Text.RegularExpressions;

#nullable enable

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        private static readonly Regex NameRegex = new("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        public string Alternative { get; }

        public PropertyAttribute(string alternative)
        {
            Alternative = alternative;
        }

        public bool IsValidName()
        {
            if (string.IsNullOrWhiteSpace(Alternative))
                return false;
            if (NameRegex.IsMatch(Alternative))
                return true;
            return false;
        }
    }
}