using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Composition.Abstractions
{
    public abstract class VerifiableSourceContext : ISourceContext
    {
        protected ContextScope Scope { get; private set; }

        public VerifiableSourceContext()
        {
            Scope = new DefaultRootScope();
        }

        public abstract string ToSourceString();

        public void SetCurrentScope(ContextScope scope)
        {
            Scope = scope;
        }

        protected void VerifyCurrentScope<T>()
        {
            Scope.VerifyCurrentIntegrity<T>();
        }

        protected void EnterToNewScope(ContextScope scope)
        {
            Scope = scope;
            Scope.VerifyIntegrity();
        }

        protected void GetOutFromCurrentScope()
        {
            Scope = Scope.Close();
        }
    }
}