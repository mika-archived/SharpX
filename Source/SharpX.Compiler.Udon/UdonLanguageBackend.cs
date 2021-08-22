
using SharpX.Compiler.Composition.Attributes;
using SharpX.Compiler.Composition.Interfaces;

namespace SharpX.Compiler.Udon;

[LanguageBackend]
public class UdonLanguageBackend : ILanguageBackend
{
    public string Identifier => "Udon";

    public void Initialize(ILanguageBackendContext context)
    {
        //
        context.RegisterExtension("uasm");
    }
}
