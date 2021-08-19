using System.Collections.Immutable;

using SharpX.Compiler.CodeCleanup.Models;
using SharpX.Compiler.Composition.Attributes;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.CodeCleanup;

[SourceRewriter]
public class CodeCleanupSourceRewriter : ISourceRewriter
{
    public string Identifier => "CodeCleanup";

    private CodeCleanupOptions _options;

    public ImmutableArray<string> SupportedIdentifiers => ImmutableArray.Create("*");

    public void Initialize(ISourceRewriterContext context)
    {
        _options = CodeCleanupOptions.Create(context.ExtraOptions);

        context.RegisterRewriter(w => new CodeCleanupCSharpSyntaxRewriter(w, _options.AllowList));
    }
}