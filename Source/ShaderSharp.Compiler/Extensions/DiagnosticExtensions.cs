using Microsoft.CodeAnalysis;

namespace ShaderSharp.Compiler.Extensions
{
    public static class DiagnosticExtensions
    {
        public static string ToErrorMessage(this Diagnostic diagnostic)
        {
            var position = diagnostic.Location.GetLineSpan().StartLinePosition;
            return $"Error {diagnostic.Id}: {diagnostic.GetMessage()} at Ln {position.Line + 1}, Col {position.Character}";
        }
    }
}