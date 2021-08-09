using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpX.Compiler.Composition.Abstractions;
using SharpX.Compiler.Composition.Enums;
using SharpX.Compiler.Composition.Interfaces;
using SharpX.Compiler.Exceptions;
using SharpX.Compiler.Models.Plugin;

namespace SharpX.Compiler.Models
{
    internal class SharpXSyntaxWalker : CSharpSyntaxWalker
    {
        private readonly AssemblyContext _assembly;
        private readonly List<string> _errors;
        private readonly SemanticModel _semanticModel;
        private readonly List<string> _warnings;
        private ISourceContext _context;

        public IReadOnlyCollection<string> Errors => _errors.AsReadOnly();

        public IReadOnlyCollection<string> Warnings => _warnings.AsReadOnly();

        public SharpXSyntaxWalker(SemanticModel semanticModel, AssemblyContext context) : base(SyntaxWalkerDepth.Token)
        {
            _semanticModel = semanticModel;
            _assembly = context;
            _errors = new List<string>();
            _warnings = new List<string>();
            _context = _assembly.Default;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            InvokePreAction(WellKnownSyntax.ClassDeclarationSyntax, node);

            base.VisitClassDeclaration(node);

            InvokePostAction(WellKnownSyntax.ClassDeclarationSyntax, node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            InvokePreAction(WellKnownSyntax.StructDeclarationSyntax, node);

            base.VisitStructDeclaration(node);

            InvokePostAction(WellKnownSyntax.StructDeclarationSyntax, node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            InvokePreAction(WellKnownSyntax.InterfaceDeclarationSyntax, node);

            base.VisitInterfaceDeclaration(node);

            InvokePostAction(WellKnownSyntax.InterfaceDeclarationSyntax, node);
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            InvokePreAction(WellKnownSyntax.RecordDeclarationSyntax, node);

            base.VisitRecordDeclaration(node);

            InvokePostAction(WellKnownSyntax.InterfaceDeclarationSyntax, node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            InvokePreAction(WellKnownSyntax.FieldDeclarationSyntax, node);

            base.VisitFieldDeclaration(node);

            InvokePostAction(WellKnownSyntax.FieldDeclarationSyntax, node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            InvokePreAction(WellKnownSyntax.PropertyDeclarationSyntax, node);

            base.VisitPropertyDeclaration(node);

            InvokePostAction(WellKnownSyntax.PropertyDeclarationSyntax, node);
        }

        private void InvokePreAction(WellKnownSyntax syntax, CSharpSyntaxNode node)
        {
            var args = new LanguageSyntaxActionContext(node, _semanticModel, _context, new AddOnlyCollection<IError>(), new AddOnlyCollection<IError>(), _assembly);
            var actions = _assembly.GetPreSyntaxActions(syntax);

            try
            {
                foreach (var action in actions)
                    if (action.Predicator.Invoke(args))
                    {
                        action.Action.Invoke(args);
                        if (args.ShouldStopPropagationIncludingSiblingActions)
                            break;
                    }

                _context = args.SourceContext;
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine(e.Message);
            }
            finally
            {
                _errors.AddRange(args.Errors.Select(w => w.GetMessage()));
                _warnings.AddRange(args.Warnings.Select(w => w.GetMessage()));

                if (args.ShouldStopPropagation)
                    throw new StopPropagationException();
            }
        }

        private void InvokePostAction(WellKnownSyntax syntax, CSharpSyntaxNode node)
        {
            var args = new LanguageSyntaxActionContext(node, _semanticModel, _context, new AddOnlyCollection<IError>(), new AddOnlyCollection<IError>(), _assembly);
            var actions = _assembly.GetPostSyntaxActions(syntax);

            try
            {
                foreach (var action in actions)
                    if (action.Predicator.Invoke(args))
                    {
                        action.Action.Invoke(args);
                        if (args.ShouldStopPropagationIncludingSiblingActions)
                            break;
                    }

                _context = args.SourceContext;
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine(e.Message);
            }
            finally
            {
                _errors.AddRange(args.Errors.Select(w => w.GetMessage()));
                _warnings.AddRange(args.Warnings.Select(w => w.GetMessage()));

                if (args.ShouldStopPropagation)
                    throw new StopPropagationException();
            }
        }
    }
}