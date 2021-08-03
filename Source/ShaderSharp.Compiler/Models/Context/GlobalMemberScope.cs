using System;

using ShaderSharp.Compiler.Models.Source;

namespace ShaderSharp.Compiler.Models.Context
{
    public class GlobalMemberScope : ContextScope, IDisposable
    {
        private readonly SourceContext _context;

        private GlobalMemberScope(SourceContext context, ContextScope parentScope) : base(parentScope)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Scope = Close();
        }

        public static GlobalMemberScope Open(SourceContext context, ContextScope scope)
        {
            var self = new GlobalMemberScope(context, scope);
            self.VerifyIntegrity();

            return self;
        }

        public override void VerifyIntegrity()
        {
            if (ParentScope is not RootContextScope)
                throw new InvalidOperationException("global member declaration must be called in the top-level scope.");
        }
    }
}