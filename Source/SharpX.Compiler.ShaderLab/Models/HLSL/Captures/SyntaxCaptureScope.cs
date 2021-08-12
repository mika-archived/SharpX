using System;

using SharpX.Compiler.Composition.Enums;

namespace SharpX.Compiler.ShaderLab.Models.HLSL.Captures
{
    public class SyntaxCaptureScope : IDisposable
    {
        private readonly WellKnownSyntax _syntax;
        private readonly ShaderLabCSharpSyntaxWalker _walker;

        private SyntaxCaptureScope(ShaderLabCSharpSyntaxWalker walker, WellKnownSyntax syntax)
        {
            _walker = walker;
            _syntax = syntax;
        }

        public void Dispose()
        {
            _walker.CapturingStack.Pop();
        }

        public static SyntaxCaptureScope Create(ShaderLabCSharpSyntaxWalker walker, WellKnownSyntax syntax)
        {
            var scope = new SyntaxCaptureScope(walker, syntax);
            scope._walker.CapturingStack.Push(syntax);

            return scope;
        }
    }
}