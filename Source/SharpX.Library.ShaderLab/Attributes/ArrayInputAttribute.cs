using System;

namespace SharpX.Library.ShaderLab.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ArrayInputAttribute : Attribute
    {
        public int ArraySize { get; }

        public ArrayInputAttribute(int size)
        {
            ArraySize = size;
        }
    }
}