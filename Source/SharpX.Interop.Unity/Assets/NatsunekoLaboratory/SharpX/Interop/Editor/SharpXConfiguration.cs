using System.ComponentModel;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace NatsunekoLaboratory.SharpX.Interop
{
    public class SharpXConfiguration : ScriptableObject
    {
        [SerializeField]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AssemblyDefinitionAsset[] _assemblies;

        [SerializeField]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DefaultAsset _executable;

        [SerializeField]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DefaultAsset[] _references;

        public AssemblyDefinitionAsset[] Assemblies
        {
            get => _assemblies;
            set => _assemblies = value;
        }

        public DefaultAsset[] References
        {
            get => _references;
            set => _references = value;
        }

        public DefaultAsset Executable
        {
            get => _executable;
            set => _executable = value;
        }
    }
}