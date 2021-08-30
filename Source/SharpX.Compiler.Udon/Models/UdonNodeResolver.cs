using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis;

using UnityEngine;

using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Video.Components.Base;
using VRC.SDKBase;
using VRC.Udon.Common;
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

        private static readonly Dictionary<string, Tuple<Type, string>[]> InternalMethodArguments = new()
        {
            { "_onAnimatorIK", new[] { new Tuple<Type, string>(typeof(int), "onAnimatorIkLayerIndex") } },
            { "_onAudioFilterRead", new[] { new Tuple<Type, string>(typeof(float[]), "onAudioFilterReadData"), new Tuple<Type, string>(typeof(int), "onAudioFilterReadChannels") } },
            { "_onCollisionEnter", new[] { new Tuple<Type, string>(typeof(Collision), "onCollisionEnterOther") } },
            { "_onCollisionEnter2D", new[] { new Tuple<Type, string>(typeof(Collision2D), "onCollisionEnter2DOther") } },
            { "_onCollisionExit", new[] { new Tuple<Type, string>(typeof(Collision), "onCollisionExitOther") } },
            { "_onCollisionExit2D", new[] { new Tuple<Type, string>(typeof(Collision2D), "onCollisionExit2DOther") } },
            { "_onCollisionStay", new[] { new Tuple<Type, string>(typeof(Collision), "onCollisionStayOther") } },
            { "_onCollisionStay2D", new[] { new Tuple<Type, string>(typeof(Collision2D), "onCollisionStay2DOther") } },
            { "_onControllerColliderHit", new[] { new Tuple<Type, string>(typeof(ControllerColliderHit), "onControllerColliderHitHit") } },
            { "_onJointBreak", new[] { new Tuple<Type, string>(typeof(float), "onJointBreakBreakForce") } },
            { "_onJointBreak2D", new[] { new Tuple<Type, string>(typeof(Joint2D), "onJointBreak2DBrokenJoint") } },
            { "_onParticleCollision", new[] { new Tuple<Type, string>(typeof(GameObject), "onParticleCollisionOther") } },
            { "_onRenderImage", new[] { new Tuple<Type, string>(typeof(RenderTexture), "onRenderImageSrc"), new Tuple<Type, string>(typeof(RenderTexture), "onRenderImageDest") } },
            { "_onTriggerEnter", new[] { new Tuple<Type, string>(typeof(Collider), "onTriggerEnterOther") } },
            { "_onTriggerEnter2D", new[] { new Tuple<Type, string>(typeof(Collider2D), "onTriggerEnter2DOther") } },
            { "_onTriggerExit", new[] { new Tuple<Type, string>(typeof(Collider), "onTriggerExitOther") } },
            { "_onTriggerExit2D", new[] { new Tuple<Type, string>(typeof(Collider2D), "onTriggerExit2DOther") } },
            { "_onTriggerStay", new[] { new Tuple<Type, string>(typeof(Collider), "onTriggerStayOther") } },
            { "_onTriggerStay2D", new[] { new Tuple<Type, string>(typeof(Collider2D), "onTriggerStay2DOther") } },
            { "_onPlayerJoined", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerJoinedPlayer") } },
            { "_onPlayerLeft", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerLeftPlayer") } },
            { "_onStationEntered", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onStationEnteredPlayer") } },
            { "_onStationExited", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onStationExitedPlayer") } },
            { "_onOwnershipRequest", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onOwnershipRequestRequester"), new Tuple<Type, string>(typeof(VRCPlayerApi), "onOwnershipRequestNewOwner") } },
            { "_onPlayerTriggerEnter", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerTriggerEnterPlayer") } },
            { "_onPlayerTriggerExit", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerTriggerExitPlayer") } },
            { "_onPlayerTriggerStay", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerTriggerStayPlayer") } },
            { "_onPlayerCollisionEnter", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerCollisionEnterPlayer") } },
            { "_onPlayerCollisionExit", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerCollisionExitPlayer") } },
            { "_onPlayerCollisionStay", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerCollisionStayPlayer") } },
            { "_onPlayerParticleCollision", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerParticleCollisionPlayer") } },
            { "_onPlayerRespawn", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onPlayerRespawnPlayer") } },
            { "_onVideoError", new[] { new Tuple<Type, string>(typeof(VideoError), "onVideoErrorVideoError") } },
            { "_midiNoteOn", new[] { new Tuple<Type, string>(typeof(int), "midiNoteOnChannel"), new Tuple<Type, string>(typeof(int), "midiNoteOnNumber"), new Tuple<Type, string>(typeof(int), "midiNoteOnVelocity") } },
            { "_midiNoteOff", new[] { new Tuple<Type, string>(typeof(int), "midiNoteOffChannel"), new Tuple<Type, string>(typeof(int), "midiNoteOffNumber"), new Tuple<Type, string>(typeof(int), "midiNoteOffVelocity") } },
            { "_midiControlChange", new[] { new Tuple<Type, string>(typeof(int), "midiControlChangeChannel"), new Tuple<Type, string>(typeof(int), "midiControlChangeNumber"), new Tuple<Type, string>(typeof(int), "midiControlChangeValue") } },
            { "_inputJump", new[] { new Tuple<Type, string>(typeof(bool), "inputJumpBoolValue"), new Tuple<Type, string>(typeof(UdonInputEventArgs), "inputJumpArgs") } },
            { "_inputUse", new[] { new Tuple<Type, string>(typeof(bool), "inputUseBoolValue"), new Tuple<Type, string>(typeof(UdonInputEventArgs), "inputUseArgs") } },
            { "_inputGrab", new[] { new Tuple<Type, string>(typeof(bool), "inputGrabBoolValue"), new Tuple<Type, string>(typeof(UdonInputEventArgs), "inputGrabArgs") } },
            { "_inputDrop", new[] { new Tuple<Type, string>(typeof(bool), "inputDropBoolValue"), new Tuple<Type, string>(typeof(UdonInputEventArgs), "inputDropArgs") } },
            { "_inputMoveHorizontal", new[] { new Tuple<Type, string>(typeof(float), "inputMoveHorizontalFloatValue"), new Tuple<Type, string>(typeof(UdonInputEventArgs), "inputMoveHorizontalArgs") } },
            { "_inputMoveVertical", new[] { new Tuple<Type, string>(typeof(float), "inputMoveVerticalFloatValue"), new Tuple<Type, string>(typeof(UdonInputEventArgs), "inputMoveVerticalArgs") } },
            { "_inputLookHorizontal", new[] { new Tuple<Type, string>(typeof(float), "inputLookHorizontalFloatValue"), new Tuple<Type, string>(typeof(UdonInputEventArgs), "inputLookHorizontalArgs") } },
            { "_inputLookVertical", new[] { new Tuple<Type, string>(typeof(float), "inputLookVerticalFloatValue"), new Tuple<Type, string>(typeof(UdonInputEventArgs), "inputLookVerticalArgs") } },
            { "_onOwnershipTransferred", new[] { new Tuple<Type, string>(typeof(VRCPlayerApi), "onOwnershipTransferredPlayer") } },
            { "_onPostSerialization", new[] { new Tuple<Type, string>(typeof(SerializationResult), "onPostSerializationResult") } }
        };

        private UdonNodeResolver() { }

        public bool IsBuiltinEvent(string name)
        {
            if (_builtinEventLookup!.ContainsKey(name))
                return true;

            return false;
        }

        public Tuple<Type, string>[]? GetMethodCustomArgs(string name)
        {
            if (InternalMethodArguments.ContainsKey(name))
                return InternalMethodArguments[name];

            return null;
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