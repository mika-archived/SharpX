using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis;

using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Video.Components.Base;
using VRC.Udon.Editor;
using VRC.Udon.Editor.ProgramSources.UdonGraphProgram.UI;

namespace SharpX.Compiler.Udon.Models
{
    internal class UdonNodeResolver
    {
        private static readonly object LockObj = new();

        private static HashSet<string>? _nodeDefinitions;
        private static Dictionary<string, string>? _builtinEventLookup;
        private static Dictionary<string, string>? _inheritTypeMappings;

        private UdonNodeResolver() { }

        public bool IsBuiltinEvent(string name)
        {
            if (_builtinEventLookup!.ContainsKey(name) == true)
                return true;

            return false;
        }

        public string RemappedBuiltinEvent(string name)
        {
            if (IsBuiltinEvent(name))
                return _builtinEventLookup![name];
            return name;
        }

        public string SanitizeTypeName(string typeName)
        {
            return typeName.Replace(",", "")
                           .Replace(".", "")
                           .Replace("[]", "Array")
                           .Replace("&", "Ref")
                           .Replace("+", "");
        }

        public bool IsValidType(ITypeSymbol symbol, SemanticModel model)
        {
            var name = GetUdonName(symbol, model);
            return _nodeDefinitions!.Contains($"Type_{name}");
        }

        public string GetUdonName(ITypeSymbol symbol, SemanticModel model, bool isSkipBaseTypeRemapping = false)
        {
            if (!isSkipBaseTypeRemapping)
                symbol = RemapBaseType(symbol, model);

            var @extern = symbol;
            while (@extern is IArrayTypeSymbol array)
                @extern = array.ElementType;
            while (@extern is IPointerTypeSymbol pointer)
                @extern = pointer.PointedAtType;

            var @namespace = @extern.ContainingNamespace.ToDisplayString();
            if (@extern.ToDisplayString().Contains("+"))
                @namespace = @extern.ToDisplayString().Replace("+", ".").Substring(0, @extern.ToDisplayString().LastIndexOf("+", StringComparison.Ordinal));

            if (@extern.ToDisplayString() is "T" or "T[]")
                @namespace = "";

            var fullyQualifiedMetadataName = SanitizeTypeName($"{@namespace}.{@extern.Name}");

            if (@extern is INamedTypeSymbol n)
                foreach (var t in n.TypeArguments)
                    fullyQualifiedMetadataName += GetUdonName(t, model);

            if (fullyQualifiedMetadataName == "SystemCollectionsGenericListT")
                fullyQualifiedMetadataName = "ListT";
            if (fullyQualifiedMetadataName == "SystemCollectionsGenericIEnumerableT")
                fullyQualifiedMetadataName = "IEnumerableT";

            return fullyQualifiedMetadataName.Replace("VRCUdonUdonBehaviour", "VRCUdonCommonInterfacesIUdonEventReceiver");
        }

        private ITypeSymbol RemapBaseType(ITypeSymbol symbol, SemanticModel model)
        {
            var cur = symbol;
            var depth = 0;
            while (cur is IArrayTypeSymbol array)
            {
                cur = array.ElementType;
                depth++;
            }

            if (_inheritTypeMappings!.ContainsKey(cur.ToDisplayString()))
            {
                var metadata = _inheritTypeMappings[cur.ToDisplayString()];
                var sig = Enumerable.Repeat("[]", depth);
                return model.Compilation.GetTypeByMetadataName(metadata + sig)!;
            }

            return symbol;
        }

        #region Initializers

        [MemberNotNull(nameof(_builtinEventLookup), nameof(_nodeDefinitions), nameof(_inheritTypeMappings))]
        private void Init()
        {
            lock (LockObj)
            {
                if (_nodeDefinitions != null || _builtinEventLookup != null)
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
                    return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

                LoadNodeDefinitions();
                LoadInheritTypeMappings();
            }
        }

        [MemberNotNull(nameof(_builtinEventLookup), nameof(_nodeDefinitions))]
        private void LoadNodeDefinitions()
        {
            _nodeDefinitions = new HashSet<string>(UdonEditorManager.Instance.GetNodeDefinitions().Select(w => w.fullName));
            _builtinEventLookup = new Dictionary<string, string>();

            foreach (var node in UdonEditorManager.Instance.GetNodeDefinitions("Event_"))
            {
                if (node.fullName == "Event_Custom")
                    continue;

                var name = node.fullName.Substring(6);
                if (_builtinEventLookup.ContainsKey(name.ToLowerFirstChar()))
                    continue;

                _builtinEventLookup.Add(name, "_" + name.ToLowerFirstChar());
            }
        }

        private void LoadInheritTypeMappings()
        {
            var mappings = new Dictionary<string, string>();
            var types = AppDomain.CurrentDomain.GetAssemblies().First(w => w.GetName().Name == "VRCSDK3").GetTypes().Where(w => w is { Namespace: { } } && w.Namespace.StartsWith("VRC.SDK3.Components"));
            foreach (var t in types)
                if (t.BaseType != null && t.BaseType.Namespace?.StartsWith("VRC.SDKBase") == true)
                    mappings.Add(t.BaseType.FullName!, t.FullName!);

            mappings.Add(typeof(VRCUnityVideoPlayer).FullName!, typeof(BaseVRCVideoPlayer).FullName!);
            mappings.Add(typeof(VRCAVProVideoPlayer).FullName!, typeof(BaseVRCVideoPlayer).FullName!);

            _inheritTypeMappings = mappings;
        }

        #endregion

        #region Instance

        private static UdonNodeResolver? _instance;

        public static UdonNodeResolver Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UdonNodeResolver();
                    _instance.Init();
                }

                return _instance;
            }
        }

        #endregion
    }
}