using System;

namespace SharpX.Library.Udon
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldChangeCallbackAttribute : Attribute
    {
        public string CallbackPropertyName { get; }

        public FieldChangeCallbackAttribute() { }

        public FieldChangeCallbackAttribute(string targetPropertyName)
        {
            CallbackPropertyName = targetPropertyName;
        }
    }
}