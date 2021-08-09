using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using SharpX.CodeGen.ShaderLab.Extensions;

namespace SharpX.CodeGen.ShaderLab.Models
{
    public record Function(string Name, List<Signature> Signatures, string Converter)
    {
        public string ToMethodDeclaration()
        {
            var sb = new StringBuilder();

            foreach (var signature in Signatures)
                try
                {
                    if (signature.Parameters.Count > 0)
                    {
                        // TODO: It seems unlikely at the moment, but we need to consider the case that the Template and Size in the HLSL document do not match.
                        for (var i = 0; i < TypeContractConverter.CalculateOverloadCount(signature.Parameters.First()); i++)
                        {
                            var args = signature.Parameters.Select(w => $"{TypeContractConverter.GetAsCSharpTypeReference(w, i + 2)} {w.Name}").ToArray();
                            var r = TypeContractConverter.GetAsCSharpTypeReference(signature.Returns, i + 2);
                            if (r == "__input__")
                                r = TypeContractConverter.GetAsCSharpTypeReference(signature.Parameters.First(), i + 2);

                            sb.AppendLine($@"        [ShaderSharp.Compiler.Abstractions.Attributes.Function(""{Name}"")]");
                            sb.AppendLine($@"        public static extern {r} {ConvertName(Name)}({string.Join(", ", args)});");
                            sb.AppendLine();
                        }
                    }
                    else
                    {
                        var r = TypeContractConverter.GetAsCSharpTypeReference(signature.Returns);

                        sb.AppendLine($@"        [ShaderSharp.Compiler.Abstractions.Attributes.Function(""{Name}"")]");
                        sb.AppendLine($@"        public static extern {r} {ConvertName(Name)}();");
                        sb.AppendLine();
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

            return sb.ToString();
        }

        private (string, string, string) CalcTypeContracts(List<Parameter> parameters)
        {
            var arguments = new List<string>();
            var typeArguments = new List<string>();
            var genericsContract = new List<string>();

            foreach (var parameter in parameters)
            {
                var t = TypeContractConverter.GetAsCSharpTypeReference(parameter);
                if (TypeContractConverter.ShouldUseGenericDefinition(parameter))
                {
                    var arg = $"T{arguments.Count + 1}";
                    arguments.Add($"{arg} {parameter.Name}");
                    typeArguments.Add(arg);
                    genericsContract.Add($"{arg} : {t}");
                }
                else
                {
                    arguments.Add($"{t} {parameter.Name}");
                }
            }

            return (
                string.Join(", ", arguments),
                typeArguments.Count > 0 ? $"<{string.Join(", ", typeArguments)}>" : "",
                genericsContract.Count > 0 ? $"where {string.Join("where ", genericsContract)}" : ""
            );
        }

        private string ConvertName(string name)
        {
            switch (Converter)
            {
                case "UpperCamelCase":
                    return name.ToUpperCamelCase();

                case "LowerCamelCase":
                    return name.ToLowerCamelCase();

                default:
                    return name;
            }
        }
    }
}