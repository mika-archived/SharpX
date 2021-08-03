using System;

namespace ShaderSharp.Compiler.Models.Context
{
    public abstract class ContextScope
    {
        protected ContextScope ParentScope { get; }

        protected ContextScope(ContextScope parentScope)
        {
            ParentScope = parentScope;
        }

        public ContextScope Close()
        {
            return ParentScope;
        }

        public abstract void VerifyIntegrity();

        public void VerifyIntegrity<T>()
        {
            if (typeof(T) != GetType())
                throw new InvalidOperationException("Failed to verify integrity by type");
        }
    }
}