using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpX.CodeGen.ShaderLab.Models
{
    public class FunctionDefinitionParser : IDisposable
    {
        private static readonly Regex TypeConstraintRegex = new("(?<inout>\\[(in|out)\\])?(?<param>.*) is (?<template>(scalar|vector|matrix|object)) implements (?<type>\\w+)( has (?<element>\\w+) elements)?", RegexOptions.Compiled);
        private static readonly Regex SignatureRegex = new("\\((?<parameters>.*)\\) => (?<return>.*)", RegexOptions.Compiled);

        private readonly StreamReader _sr;

        public string? Namespace { get; private set; }

        public string? Class { get; private set; }

        public string? Include { get; private set; }

        public string Converter { get; private set; } = "None";

        public List<Function> Functions { get; } = new();

        public FunctionDefinitionParser(StreamReader sr)
        {
            _sr = sr;
        }

        public void Dispose()
        {
            _sr.Dispose();
        }

        public void Parse()
        {
            var state = ParserState.IntoTopLevel;
            var statements = new List<string>();
            var functions = new List<Function>();

            Function? function = null;
            string? line;
            while ((line = _sr.ReadLine()) != null)
            {
                line = line.Trim();

                switch (line)
                {
                    case { } when line.StartsWith("//"):
                        break;

                    case { } when line.StartsWith("#"):
                        ParseDirective(line.Substring(1));
                        break;

                    case { } when line.StartsWith("function"):
                        state = ParserState.IntoFunction;
                        function = new Function(line.Split(' ')[1], new List<Signature>(), Converter);
                        break;

                    case { } when line.StartsWith("signatures"):
                        state = ParserState.IntoSignatures;
                        break;

                    case { } when line.StartsWith("{"):
                        break;

                    case { } when line.StartsWith("}"):
                        switch (state)
                        {
                            case ParserState.IntoTopLevel:
                                throw new InvalidOperationException();

                            case ParserState.IntoFunction:
                                if (function != null)
                                {
                                    functions.Add(function);
                                    function = null;
                                }

                                state = ParserState.IntoTopLevel;
                                break;

                            case ParserState.IntoSignatures:
                                if (function != null)
                                    function.Signatures.AddRange(ParseSignatures(statements));

                                state = ParserState.IntoFunction;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        statements.Clear();
                        break;

                    default:
                        statements.Add(line!);
                        break;
                }
            }

            Functions.Clear();
            Functions.AddRange(functions);
        }

        private void ParseDirective(string directive)
        {
            static (string, string) Destruct(string statement)
            {
                var arr = statement.Split(' ').Select(w => w.Trim()).ToArray();
                return (arr[0], string.Concat(arr.Skip(1)));
            }

            var (category, value) = Destruct(directive);

            switch (category)
            {
                case "namespace":
                    Namespace = value;
                    break;

                case "class":
                    Class = value;
                    break;

                case "converter":
                    Converter = value;
                    break;

                case "include":
                    Include = value;
                    break;
            }
        }

        private List<Signature> ParseSignatures(List<string> statements)
        {
            var signatures = new List<Signature>();

            static Parameter ParseParameter(string str)
            {
                if (!TypeConstraintRegex.IsMatch(str))
                    return new Parameter(null, null, str, null, false);

                var contract = TypeConstraintRegex.Match(str);
                var t = new Parameter(contract.Groups["param"].Value, contract.Groups["template"].Value, contract.Groups["type"].Value, null, contract.Groups["inout"].Value == "[out]");
                if (!string.IsNullOrWhiteSpace(contract.Groups["element"].Value))
                    t = t with { Element = contract.Groups["element"].Value };

                return t;
            }

            foreach (var statement in statements)
            {
                if (!SignatureRegex.IsMatch(statement))
                    continue;

                var matched = SignatureRegex.Match(statement);
                var signature = new Signature(new List<Parameter>(), ParseParameter(matched.Groups["return"].Value));

                if (!string.IsNullOrWhiteSpace(matched.Groups["parameters"].Value))
                {
                    var parameters = matched.Groups["parameters"].Value.Split(',').Select(w => w.Trim());
                    foreach (var parameter in parameters)
                    {
                        var t = ParseParameter(parameter);
                        if (t != null)
                            signature.Parameters.Add(t);
                    }
                }

                signatures.Add(signature);
            }

            return signatures;
        }

        private enum ParserState
        {
            IntoTopLevel,

            IntoFunction,

            IntoSignatures
        }
    }
}