namespace ShaderSharp.Compiler.Models.Source.Structure.Interfaces
{
    public interface IHeaderComponent : IComponent
    {
        int Priority { get; }
    }
}