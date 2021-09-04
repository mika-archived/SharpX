using System;

namespace SharpX.CLI.Parser.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class OptionAttribute : Attribute
    {
        public string Name { get; }

        public OptionAttribute(string name)
        {
            Name = name;
        }
    }
}