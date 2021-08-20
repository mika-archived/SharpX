using System;
using System.ComponentModel;

using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EnumAttribute : InspectorAttribute
    {
        public string FullyQualifiedName { get; }

        public Type T { get; }

        public dynamic Symbol { get; }

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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public EnumAttribute(object t)
        {
            Symbol = t; // THIS IS A WORKAROUND FOR TypeConstantKind.Type.
        }

        public override string ToSourceString()
        {
            if (!string.IsNullOrWhiteSpace(FullyQualifiedName))
                return $"Enum({FullyQualifiedName})";
            if (T != null)
                return $"Enum({T.FullName})";
            return $"Enum({string.Join(", ", Values)})";
        }
    }
}