using System.Collections.Generic;

namespace ShaderSharp.Compiler.Models.Source.Structure.Interfaces
{
    public interface IStructDeclarationComponent : IComponent
    {
        void AddMemberDeclaration(string str);

        void AddMemberDeclaration(string t, string name, params KeyValuePair<string, string>[] extras);
    }
}