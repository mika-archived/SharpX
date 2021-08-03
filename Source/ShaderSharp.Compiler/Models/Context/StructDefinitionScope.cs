using System;

namespace ShaderSharp.Compiler.Models.Context
{
    internal class StructDefinitionScope : ContextScope
    {
        public StructDefinitionScope(ContextScope parentScope) : base(parentScope) { }

        public override void VerifyIntegrity()
        {
            if (ParentScope is not RootContextScope)
                throw new InvalidOperationException("struct declarations must be always in the top-level scope.");
        }
    }
}