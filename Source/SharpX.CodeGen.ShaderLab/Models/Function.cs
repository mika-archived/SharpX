using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using SharpX.CodeGen.ShaderLab.Extensions;

namespace SharpX.CodeGen.ShaderLab.Models
{
    public record Function(string Name, List<Signature> Signatures, string Converter)
    {
        private static readonly Regex ReturnIsSameAsInput = new("same\\((?<arg>.*)\\)", RegexOptions.Compiled);

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
                            var args = signature.Parameters.Select(w => $"{(w.isOut ? "out " : "")}{TypeContractConverter.GetAsCSharpTypeReference(w, i + 2)} {w.Name}").ToArray();
                            var r = TypeContractConverter.GetAsCSharpTypeReference(signature.Returns, i + 2);
                            if (r == "__input__")
                                r = TypeContractConverter.GetAsCSharpTypeReference(signature.Parameters.First(), i + 2);
                            if (ReturnIsSameAsInput.IsMatch(r))
                            {
                                var a = ReturnIsSameAsInput.Match(r).Groups["arg"].Value;
                                r = TypeContractConverter.GetAsCSharpTypeReference(signature.Parameters.Single(w => w.Name == a), i + 2);
                            }

                            sb.AppendLine($@"        [SharpX.Library.ShaderLab.Attributes.Function(""{Name}"")]");
                            sb.AppendLine($@"        public static extern {r} {ConvertName(Name)}({string.Join(", ", args)});");
                            sb.AppendLine();
                        }
                    }
                    else
                    {
                        var r = TypeContractConverter.GetAsCSharpTypeReference(signature.Returns);

                        sb.AppendLine($@"        [SharpX.Library.ShaderLab.Attributes.Function(""{Name}"")]");
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