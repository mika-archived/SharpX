using System;
using System.Reflection;

namespace SharpX.Compiler.Composition.Abstractions
{
    public abstract class DisposableContextScope : ContextScope, IDisposable
    {
        private readonly VerifiableSourceContext _context;

        protected DisposableContextScope(VerifiableSourceContext context, ContextScope scope) : base(scope)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.SetCurrentScope(Close());
        }

        public static T Open<T>(VerifiableSourceContext context, ContextScope scope) where T : DisposableContextScope
        {
            if (Activator.CreateInstance(typeof(T), BindingFlags.CreateInstance, context, scope) is not T self)
                throw new InvalidOperationException("Failed to create/open a scope with 2 arguments.");

            self.VerifyIntegrity();
            return self;
        }
    }
}