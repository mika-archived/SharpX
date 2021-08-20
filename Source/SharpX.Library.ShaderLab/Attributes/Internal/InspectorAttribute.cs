using System;

namespace SharpX.Library.ShaderLab.Attributes.Internal
{
    public abstract class InspectorAttribute : Attribute
    {
        public abstract string ToSourceString();
    }
}