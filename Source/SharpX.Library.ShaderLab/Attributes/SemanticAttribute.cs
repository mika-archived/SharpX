using System;
using System.Text.RegularExpressions;

#nullable enable

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SemanticAttribute : Attribute
    {
        private static readonly Regex SemanticNameRegex = new("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

        public string Semantic { get; }

        public SemanticAttribute(string semantic)
        {
            Semantic = semantic;
        }

        public bool IsValidSemantics()
        {
            return SemanticNameRegex.IsMatch(Semantic);
        }
    }
}