using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EnumAttribute : Attribute
    {
        public string FullyQualifiedName { get; }

        public Type T { get; }

        public object[] Values { get; }

        public EnumAttribute(string fullyQualifiedName)
        {
            FullyQualifiedName = fullyQualifiedName;
        }

        public EnumAttribute(Type t)
        {
            T = t;
        }

        public EnumAttribute(params object[] values)
        {
            Values = values;
        }
    }
}