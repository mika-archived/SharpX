using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MaterialInputAttribute : Attribute
    {
        public string VariableName { get; }

        public MaterialInputAttribute(string name)
        {
            VariableName = name;
        }
    }
}