using System;

namespace SharpX.Library.CodeCleanup.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class MarkAttribute : Attribute { }
}