using System;

namespace ShaderSharp.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        public string Name { get; }

        public PropertyAttribute(string name)
        {
            Name = name;
        }
    }
}