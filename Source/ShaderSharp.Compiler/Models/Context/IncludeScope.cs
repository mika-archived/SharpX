using System;

using ShaderSharp.Compiler.Models.Source;

namespace ShaderSharp.Compiler.Models.Context
{
    public class IncludeScope : ContextScope, IDisposable
    {
        private readonly SourceContext _context;

        private IncludeScope(SourceContext context, ContextScope parentScope) : base(parentScope)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Scope = Close();
        }

        public static IncludeScope Open(SourceContext context, ContextScope scope)
        {
            var self = new IncludeScope(context, scope);
            self.VerifyIntegrity();

            return self;
        }

        public override void VerifyIntegrity()
        {
            if (ParentScope is not RootContextScope)
                throw new InvalidOperationException("#include must be called in the top-level scope.");
        }
    }
}