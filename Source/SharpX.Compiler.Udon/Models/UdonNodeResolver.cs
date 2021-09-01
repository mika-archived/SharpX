using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using SharpX.Compiler.Udon.Enums;
using SharpX.Library.Udon;

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
        private static readonly Regex OperatorRegex = new("__op_(.+?)__", RegexOptions.Compiled);

        private static HashSet<string>? _nodeDefinitions;
        private static Dictionary<string, string>? _builtinEventLookup;
        private static Dictionary<string, string>? _inheritTypeMappings;


        private static readonly Dictionary<string, string> BuiltinTypes = new()
        {
            { "void", typeof(void).FullName! },
            { "string", typeof(string).FullName! },
            { "int", typeof(int).FullName! },
            { "uint", typeof(uint).FullName! },
            { "long", typeof(long).FullName! },
            { "ulong", typeof(ulong).FullName! },
            { "short", typeof(short).FullName! },
            { "ushort", typeof(ushort).FullName! },
            { "char", typeof(char).FullName! },
            { "bool", typeof(bool).FullName! },
            { "byte", typeof(byte).FullName! },
            { "sbyte", typeof(sbyte).FullName! },
            { "float", typeof(float).FullName! },
            { "double", typeof(double).FullName! },
            { "decimal", typeof(decimal).FullName! },
            { "object", typeof(object).FullName! }
        };

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
            var name = GetUdonTypeName(symbol, model);
            if (name == "SystemVoid")
                return true;
            if (IsAllowed(symbol)) // no type checking for user-defined types
                return true;
            if (symbol.Equals(model.Compilation.GetTypeByMetadataName(typeof(SharpXUdonBehaviour).FullName!), SymbolEqualityComparer.Default))
                return true;
            return _nodeDefinitions!.Contains($"Type_{name}");
        }

        public string GetUdonTypeName(Type t, SemanticModel model, bool isSkipBaseTypeRemapping = false)
        {
            return GetUdonTypeName(model.Compilation.GetTypeByMetadataName(t.FullName!)!, model, isSkipBaseTypeRemapping);
        }

        public string GetUdonTypeName(ITypeSymbol symbol, SemanticModel model, bool isSkipBaseTypeRemapping = false)
        {
            if (!isSkipBaseTypeRemapping)
                symbol = RemapBaseType(symbol, model);

            var @extern = symbol;
            while (@extern is IArrayTypeSymbol array)
                @extern = array.ElementType;
            while (@extern is IPointerTypeSymbol pointer)
                @extern = pointer.PointedAtType;

            var @namespace = @extern.ContainingNamespace.ToDisplayString();
            if (@extern.ContainingType != null)
            {
                var declaringNamespace = "";
                var t = @extern.ContainingType;

                while (t != null)
                {
                    declaringNamespace = $"{@extern.ContainingType.Name}.{declaringNamespace}";
                    t = t.ContainingType;
                }

                @namespace += $".{declaringNamespace}";
            }

            if (@extern.ToDisplayString() is "T" or "T[]")
                @namespace = "";

            var fullyQualifiedMetadataName = SanitizeTypeName($"{@namespace}.{@extern.Name}");

            if (@extern is INamedTypeSymbol n)
                foreach (var t in n.TypeArguments)
                    fullyQualifiedMetadataName += GetUdonTypeName(t, model);

            if (fullyQualifiedMetadataName == "SystemCollectionsGenericListT")
                fullyQualifiedMetadataName = "ListT";
            if (fullyQualifiedMetadataName == "SystemCollectionsGenericIEnumerableT")
                fullyQualifiedMetadataName = "IEnumerableT";
            if (BuiltinTypes.ContainsKey(fullyQualifiedMetadataName))
                fullyQualifiedMetadataName = BuiltinTypes[fullyQualifiedMetadataName];

            return fullyQualifiedMetadataName.Replace("VRCUdonUdonBehaviour", "VRCUdonCommonInterfacesIUdonEventReceiver");
        }

        public bool IsValidMethod(IMethodSymbol method, SemanticModel model)
        {
            if (IsAllowed(method)) // no method checking for user-defined types
                return true;

            var signature = GetUdonMethodName(method, model);
            return IsValidMethod(signature);
        }

        public bool IsValidMethod(string signature)
        {
            return _nodeDefinitions!.Contains(signature);
        }

        public bool IsAllowed(ISymbol symbol)
        {
            if (symbol is IArrayTypeSymbol array)
                return IsAllowed(array.ElementType);
            return symbol.OriginalDefinition.Locations.Any(w => w.IsInSource);
        }

        public bool IsAllowedEnum(ISymbol symbol, SemanticModel model)
        {
            return IsValidType(symbol.ContainingType, model) && symbol.ContainingType.TypeKind == TypeKind.Enum;
        }

        public string GetUdonMethodName(IMethodSymbol method, SemanticModel model)
        {
            var symbol = RemapBaseType(method.ContainingType, model);

            var isUdonBehaviour = symbol.Equals(model.Compilation.GetTypeByMetadataName(typeof(SharpXUdonBehaviour).FullName!), SymbolEqualityComparer.Default);
            if (symbol.BaseType?.Equals(model.Compilation.GetTypeByMetadataName(typeof(SharpXUdonBehaviour).FullName!), SymbolEqualityComparer.Default) == true)
                isUdonBehaviour = true;

            var functionNamespace = SanitizeTypeName(GetUdonTypeName(symbol, model)).Replace("VRCUdonUdonBehaviour", "VRCUdonCommonInterfacesIUdonEventReceiver");
            if (isUdonBehaviour && functionNamespace == typeof(SharpXUdonBehaviour).FullName!.Replace(".", ""))
                functionNamespace = "VRCUdonCommonInterfacesIUdonEventReceiver";

            var methodName = $"__{method.Name.Trim('_').TrimStart('.')}";
            if (isUdonBehaviour && methodName == "__VRCInstantiate")
            {
                functionNamespace = "VRCInstantiate";
                methodName = "__Instantiate";
            }

            var paramsStr = "";
            if (method.Parameters.Length > 0)
            {
                paramsStr += "_";

                foreach (var parameter in method.Parameters)
                    paramsStr += $"_{GetUdonTypeName(parameter.Type, model, true)}";
            }
            else if (method.Name == ".ctor")
            {
                paramsStr = "__";
            }

            var returnStr = method.Name == ".ctor" ? $"__{GetUdonTypeName(method.ReturnType, model, true)}" : $"__{GetUdonTypeName(method.ReturnType, model)}";

            return $"{functionNamespace}.{methodName}{paramsStr}{returnStr}";
        }

        public bool IsValidPropertyAccessor(IPropertySymbol property, SemanticModel model, bool isGetter)
        {
            if (property.Locations.First().IsInSource) // no accessor checking for user-defined types
                return true;

            var signature = GetUdonPropertyAccessorName(property, model, isGetter);
            return _nodeDefinitions!.Contains(signature);
        }

        public string GetUdonPropertyAccessorName(IPropertySymbol property, SemanticModel model, bool isGetter)
        {
            var symbol = RemapBaseType(property.ContainingType, model);

            var functionNamespace = SanitizeTypeName(GetUdonTypeName(symbol, model)).Replace("VRCUdonUdonBehaviour", "VRCUdonCommonInterfacesIUdonEventReceiver");
            var methodName = $"__{(isGetter ? "get" : "set")}_{property.Name.Trim('_')}";
            var paramStr = $"__{GetUdonTypeName(property.Type, model)}";
            return $"{functionNamespace}.{methodName}{paramStr}";
        }

        public bool IsValidPropertyAccessor(IFieldSymbol property, SemanticModel model, bool isGetter)
        {
            var signature = GetUdonPropertyAccessorName(property, model, isGetter);
            return _nodeDefinitions!.Contains(signature);
        }

        public string GetUdonPropertyAccessorName(IFieldSymbol property, SemanticModel model, bool isGetter)
        {
            var symbol = RemapBaseType(property.ContainingType, model);

            var functionNamespace = SanitizeTypeName(GetUdonTypeName(symbol, model)).Replace("VRCUdonUdonBehaviour", "VRCUdonCommonInterfacesIUdonEventReceiver");
            var methodName = $"__{(isGetter ? "get" : "set")}_{property.Name.Trim('_')}";
            var paramStr = $"__{GetUdonTypeName(property.Type, model)}";
            return $"{functionNamespace}.{methodName}{paramStr}";
        }

        public string GetOperator(Type symbol, SemanticModel model, BuiltinOperators operators)
        {
            var t = GetUdonTypeName(symbol, model);
            return $"{t}.__op_{operators}__._{t}__{t}";
        }

        public string RemappedToBuiltinOperator(IMethodSymbol symbol, SemanticModel model, SyntaxKind kind)
        {
            string ToSignature(BuiltinOperators @operator)
            {
                var name = GetUdonMethodName(symbol, model);
                return OperatorRegex.Replace(name, $"__op_{@operator}__");
            }

            switch (kind)
            {
                case SyntaxKind.AddExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.PlusEqualsToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PostIncrementExpression:
                    return ToSignature(BuiltinOperators.Addition);

                case SyntaxKind.SubtractExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MinusEqualsToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.PostDecrementExpression:
                    return ToSignature(BuiltinOperators.Subtraction);

                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.AsteriskEqualsToken:
                    return ToSignature(BuiltinOperators.Multiply);

                case SyntaxKind.DivideExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.SlashEqualsToken:
                    return ToSignature(BuiltinOperators.Division);

                case SyntaxKind.ModuloExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.PercentEqualsToken:
                    return ToSignature(BuiltinOperators.Remainder);

                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.MinusToken:
                    return ToSignature(BuiltinOperators.UnaryMinus); // negation????

                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.LessThanLessThanEqualsToken:
                    return ToSignature(BuiltinOperators.LeftShift);

                case SyntaxKind.RightShiftExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.GreaterThanGreaterThanEqualsToken:
                    return ToSignature(BuiltinOperators.RightShift);

                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.AmpersandEqualsToken:
                    return ToSignature(BuiltinOperators.LogicalAnd);

                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.BarEqualsToken:
                    return ToSignature(BuiltinOperators.LogicalOr);

                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.TildeToken:
                    return ToSignature(BuiltinOperators.BitwiseNot);

                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.CaretEqualsToken:
                    return ToSignature(BuiltinOperators.LogicalXor);

                case SyntaxKind.LogicalOrExpression:
                    return ToSignature(BuiltinOperators.ConditionalOr);

                case SyntaxKind.LogicalAndExpression:
                    return ToSignature(BuiltinOperators.ConditionalAnd);

                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.ExclamationToken:
                    return ToSignature(BuiltinOperators.UnaryNegation);

                case SyntaxKind.EqualsExpression:
                    return ToSignature(BuiltinOperators.Equality);

                case SyntaxKind.GreaterThanExpression:
                    return ToSignature(BuiltinOperators.GreaterThan);

                case SyntaxKind.GreaterThanOrEqualExpression:
                    return ToSignature(BuiltinOperators.GreaterThanOrEqual);

                case SyntaxKind.LessThanExpression:
                    return ToSignature(BuiltinOperators.LessThan);

                case SyntaxKind.LessThanOrEqualExpression:
                    return ToSignature(BuiltinOperators.LessThanOrEqual);

                case SyntaxKind.NotEqualsExpression:
                    return ToSignature(BuiltinOperators.Inequality);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                return model.Compilation.GetTypeByMetadataName(metadata + string.Join("", sig))!;
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