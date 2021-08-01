using System;

namespace ShaderSharp.Compiler.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        public string Name { get; }

        public PropertyAttribute(string name)
        {
            Name = name;
        }
    }
}