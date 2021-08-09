using System;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;

namespace SharpX.Compiler.Extensions
{
    public static class AttributeDataExtensions
    {
        public static T AsAttributeInstance<T>(this AttributeData obj) where T : Attribute
        {
            var t = typeof(T);
            var constructors = t.GetConstructors();
            var constructorArguments = obj.ConstructorArguments;
            var constructor = constructors.FirstOrDefault(w => w.GetParameters().Length == constructorArguments.Length);

            if (constructor == null)
                return default;

            var instance = (T) constructor.Invoke(constructorArguments.Select(w => w.Value).ToArray());

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
    }
}