using System;

#nullable enable

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SemanticAttribute : Attribute
    {
        public string Semantic { get; }

        public SemanticAttribute(string semantic)
        {
            Semantic = semantic;
        }
    }
}