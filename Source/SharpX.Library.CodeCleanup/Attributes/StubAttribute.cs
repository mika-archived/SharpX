using System;

namespace SharpX.Library.CodeCleanup.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Method)]
    public class StubAttribute : Attribute { }
}