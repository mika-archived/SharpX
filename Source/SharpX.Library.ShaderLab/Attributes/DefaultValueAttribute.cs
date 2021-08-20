using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DefaultValueAttribute : Attribute
    {
        public object Parameter { get; }

        public DefaultValueAttribute(object parameter)
        {
            Parameter = parameter;
        }
    }
}