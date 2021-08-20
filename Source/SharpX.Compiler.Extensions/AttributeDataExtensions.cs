using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;

namespace SharpX.Compiler.Extensions
{
    public static class AttributeDataExtensions
    {
        private static readonly Dictionary<string, string> TypeMappings = new()
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

        private static string PrettyTypeName(string fullyQualifiedNameIncludeGlobal)
        {
            var fullyQualifiedName = fullyQualifiedNameIncludeGlobal.Replace("global::", "");
            if (TypeMappings.ContainsKey(fullyQualifiedName))
                return TypeMappings[fullyQualifiedName];
            return fullyQualifiedName;
        }

        private static object ConvertType(TypedConstant c)
        {
            if (c.Kind == TypedConstantKind.Array)
                return c.Values.Select(w => w.Value);
            if (c.Kind == TypedConstantKind.Type)
                return ((INamedTypeSymbol)c.Value!).ToDisplayString();

            return c.Value!;
        }

        public static T? AsAttributeInstance<T>(this AttributeData obj) where T : Attribute
        {
            var t = typeof(T);
            return AsAttributeInstance(obj, t) as T;
        }

        public static object? AsAttributeInstance(this AttributeData obj, Type t)
        {
            if (!t.IsAssignableTo(typeof(Attribute)))
                return default;

            var constructors = t.GetConstructors();
            var constructorArguments = obj.ConstructorArguments;

            // TODO: Type Checking
            var candidates = constructors.Where(w => w.GetParameters().Length == constructorArguments.Length).ToList();
            if (candidates.Count == 0)
                return default;

            var argumentValues = constructorArguments.Select(ConvertType).ToArray();
            var argumentTyping = constructorArguments.Select(w => PrettyTypeName(w.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))).ToArray();
            var constructor = candidates.FirstOrDefault(w =>
            {
                foreach (var (parameter, index) in w.GetParameters().Select((v, i) => (v, i)))
                {
                    if (parameter.ParameterType.FullName == typeof(object).FullName)
                        return true; // always true

                    if (parameter.ParameterType.FullName == "System.String" && argumentTyping[index] == "System.Type")
                        return true; // THIS IS A WORKAROUND FOR TypeConstantKind.Type.

                    if (parameter.ParameterType.FullName == "System.Type" && argumentTyping[index] == "System.Type")
                        return false; // THIS IS A WORKAROUND FOR TypeConstantKind.Type.

                    if (parameter.ParameterType.FullName!.Replace("+", ".") != argumentTyping[index])
                        return false;
                }

                return true;
            });

            if (constructor == null)
                return default;

            try
            {
                var instance = constructor.Invoke(argumentValues);

                if (obj.NamedArguments.Length == 0)
                    return instance;

                foreach (var (name, constant) in obj.NamedArguments)
                {
                    var property = t.GetProperty(name, BindingFlags.Public);
                    if (property == null)
                        continue;

                    property.SetValue(instance, constant.Value);
                }

                return instance;
            }
            catch (Exception e) when (Debugger.IsAttached)
            {
                Debug.WriteLine(e.Message);
            }

            return default;
        }
    }
}