using System;
using System.Linq;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EnumAttribute : InspectorAttribute
    {
        public string FullyQualifiedName { get; }

        public Type T { get; }

        public object[] Values { get; }

        public EnumAttribute(string fullyQualifiedName)
        {
            FullyQualifiedName = fullyQualifiedName;
        }

        public EnumAttribute(params object[] values)
        {
            Values = values;
        }

        public EnumAttribute(Type t)
        {
            T = t;
        }

        public override string ToSourceString()
        {
            if (!string.IsNullOrWhiteSpace(FullyQualifiedName))
                return $"Enum({FullyQualifiedName})";
            if (T != null)
                return $"Enum({T.FullName})";
            return $"Enum({string.Join(", ", Values.Select(w => w.ToString()))})";
        }
    }
}