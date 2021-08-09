using System;

namespace SharpX.Compiler.Composition.Abstractions
{
    public abstract class ContextScope
    {
        protected ContextScope? ParentScope { get; }

        protected ContextScope(ContextScope scope)
        {
            ParentScope = scope;
        }

        internal ContextScope()
        {
            ParentScope = null;
        }

        public ContextScope Close()
        {
            if (ParentScope is null)
                throw new InvalidOperationException("It is not possible to expand the scope width any further.");
            return ParentScope;
        }

        public virtual void VerifyIntegrity() { }

        public virtual void VerifyCurrentIntegrity<T>()
        {
            if (typeof(T) != GetType())
                throw new InvalidOperationException("Failed to verify integrity by type");
        }

        public virtual void VerifyParentIntegrity() { }

        public virtual void VerifyParentIntegrity<T>(string message = "Failed to verify integrity by type to be parent")
        {
            if (typeof(T) != ParentScope?.GetType())
                throw new InvalidOperationException(message);
        }
    }
}