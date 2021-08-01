using System;

namespace ShaderSharp.Compiler.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ComponentAttribute : Attribute
    {
        public string Name { get; }

        public ComponentAttribute(string name)
        {
            Name = name;
        }
    }
}