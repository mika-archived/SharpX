using System;
using System.Text.RegularExpressions;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionAttribute : Attribute
    {
        private static readonly Regex NameRegex = new("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        public string Alternative { get; }

        public FunctionAttribute(string alternative)
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