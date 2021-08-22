using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

using SharpX.Compiler.Extensions;
using SharpX.Library.ShaderLab.Attributes;
using SharpX.Library.ShaderLab.Attributes.Internal;

namespace SharpX.Compiler.ShaderLab.Models.Shader
{
    internal class ShaderBuilder
    {
        private static readonly List<string> AllowedAttributes = new()
        {
            typeof(CustomInspectorAttributeAttribute).FullName!,
            typeof(EnumAttribute).FullName!,
            typeof(GammaAttribute).FullName!,
            typeof(HDRAttribute).FullName!,
            typeof(HeaderAttribute).FullName!,
            typeof(HideInInspectorAttribute).FullName!,
            typeof(MainColorAttribute).FullName!,
            typeof(MainTextureAttribute).FullName!,
            typeof(NormalAttribute).FullName!,
            typeof(NoScaleOffsetAttribute).FullName!,
            typeof(PerRenderDataAttribute).FullName!,
            typeof(PowerSliderAttribute).FullName!,
            typeof(SpaceAttribute).FullName!
        };

        private readonly ShaderSourceContext _context;
        private readonly dynamic _instance;
        private readonly SemanticModel _model;

        public ShaderBuilder(ShaderSourceContext context, SemanticModel semanticModel, dynamic instance)
        {
            _context = context;
            _model = semanticModel;
            _instance = instance;
        }

        public void Build()
        {
            _context.Name = _instance.Name;
            _context.CustomEditor = _instance.CustomEditor?.FullName;
            _context.Fallback = _instance.Fallback;

            BuildProperties();
            BuildSubShaders();
        }

        #region SubShaders

        private void BuildSubShaders()
        {
            var shaders = _instance.SubShaders;

            foreach (var shader in shaders)
            {
                var statement = new SubShaderStructure
                {
                    Lod = shader.Lod,
                    GrabPass = shader.GrabPass
                };

                foreach (var tag in shader.Tags)
                    statement.Tags.Add(tag.Key, tag.Value);

                foreach (var pass in shader.Pass)
                    statement.Pass.Add(BuildShaderPass(pass));

                _context.SubShaders.Add(statement);
            }
        }

        private ShaderPassStructure BuildShaderPass(dynamic pass)
        {
            var s = new ShaderPassStructure
            {
                AlphaToMask = pass.AlphaToMask,
                Blend = pass.Blend,
                BlendOp = pass.BlendOp,
                ColorMask = pass.ColorMask,
                Culling = pass.Cull,
                Name = pass.Name,
                Offset = pass.Offset,
                ZTest = pass.ZTest,
                ZWrite = pass.ZWrite
            };

            foreach (var pragma in pass.Pragmas)
                s.Pragmas.Add(pragma.Key, pragma.Value);

            foreach (var tag in pass.Tags) 
                s.Tags.Add(tag.Key, tag.Value);

            return s;
        }

        #endregion

        #region Properties

        private void BuildProperties()
        {
            // Probably due to a bug in dotnet/runtime, when I try to get a value via dynamic or reflection, throws an exception. But the Visual Studio Debugger can display it.
            Type properties = _instance.Properties!;
            var t = _model.Compilation.GetTypeByMetadataName(properties.FullName!);
            if (t == null)
                return;

            foreach (var symbol in t.GetMembers().Where(w => w is IFieldSymbol or IPropertySymbol && !w.Name.Contains("k__BackingField")))
                switch (symbol)
                {
                    case IFieldSymbol f:
                        ProcessField(f);
                        break;

                    case IPropertySymbol p:
                        ProcessProperty(p);
                        break;
                }
        }

        private void ProcessProperty(IPropertySymbol symbol)
        {
            if (symbol.HasAttribute<ExternalAttribute>(_model))
                return;
            if (!symbol.HasAttribute<GlobalMemberAttribute>(_model))
                return;

            var type = symbol.Type.Name;
            if (symbol.HasAttribute<RangeAttribute>(_model))
                type = GetTypedRange(symbol);
            var name = symbol.HasAttribute<PropertyAttribute>(_model) ? symbol.GetAttribute<PropertyAttribute>(_model)!.Alternative : symbol.Name;
            var displayName = symbol.HasAttribute<DisplayNameAttribute>(_model) ? symbol.GetAttribute<DisplayNameAttribute>(_model)!.DisplayName : symbol.Name;
            var @default = GetDefaultForType(symbol.GetAttribute<DefaultValueAttribute>(_model)?.Parameter ?? "");
            var inspectorAttributes = symbol.GetAttributes().Select(ConvertToInspectorAttribute).Where(w => w != null).Select(w => w.ToSourceString()).ToArray();

            _context.Properties.Add(new ShaderProperty(type, name, displayName, @default, inspectorAttributes));
        }

        private void ProcessField(IFieldSymbol symbol)
        {
            if (symbol.HasAttribute<ExternalAttribute>(_model))
                return;
            if (!symbol.HasAttribute<GlobalMemberAttribute>(_model))
                return;

            var type = symbol.Type.Name;
            if (symbol.HasAttribute<RangeAttribute>(_model))
                type = GetTypedRange(symbol);
            var name = symbol.HasAttribute<PropertyAttribute>(_model) ? symbol.GetAttribute<PropertyAttribute>(_model)!.Alternative : symbol.Name;
            var displayName = symbol.HasAttribute<DisplayNameAttribute>(_model) ? symbol.GetAttribute<DisplayNameAttribute>(_model)!.DisplayName : symbol.Name;
            var @default = GetDefaultForType(symbol.GetAttribute<DefaultValueAttribute>(_model)?.Parameter ?? "");
            var inspectorAttributes = symbol.GetAttributes().Select(ConvertToInspectorAttribute).Where(w => w != null).Select(w => w.ToSourceString()).ToArray();

            _context.Properties.Add(new ShaderProperty(type, name, displayName, @default, inspectorAttributes));
        }

        private static string? GetDefaultForType(object parameter)
        {
            if (parameter is bool b)
                return b ? "1" : "0";
            return parameter.ToString();
        }

        private string GetTypedRange(ISymbol symbol)
        {
            var attr = symbol.GetAttribute<RangeAttribute>(_model)!;
            return $"Range({attr.Min}, {attr.Max})";
        }

        private static InspectorAttribute? ConvertToInspectorAttribute(AttributeData data)
        {
            var fullyQualifiedName = data.AttributeClass!.ToDisplayString();
            if (!AllowedAttributes.Contains(fullyQualifiedName))
                return null;

            var obj = data.AsAttributeInstance(typeof(InspectorAttribute).Assembly.GetType(fullyQualifiedName)!);
            return obj as InspectorAttribute;
        }

        #endregion
    }
}