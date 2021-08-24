using System;

namespace SharpX.Library.ShaderLab.Attributes.Internal
{
    public abstract class SourcePartAttribute : Attribute
    {
        public abstract string ToSourcePart();
    }
}