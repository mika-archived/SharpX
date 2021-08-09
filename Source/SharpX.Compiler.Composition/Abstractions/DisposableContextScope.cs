using System;

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
            var constructor = typeof(T).GetConstructor(new[] { typeof(VerifiableSourceContext), typeof(ContextScope) });
            if (constructor == null)
                throw new InvalidOperationException("Failed to create and open a scope with default 2 arguments.");

            var self = (T) constructor.Invoke(new object?[] { context, scope });
            self.VerifyIntegrity();
            return self;
        }
    }
}