using Analyzer1;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Formatting;

namespace Analyzer1
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StaticFieldsCodeFixProvider)), Shared]
    public class StaticFieldsCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(StaticFieldNamingAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
           var diagnostic = context.Diagnostics.First();
            context.RegisterCodeFix(
                CodeAction.Create(
                    CodeFixResources.GP007Fix,
                    c => AddUnderScoreSymbolAsync(context.Document, diagnostic, c),
                    CodeFixResources.GP007Fix),
                diagnostic);
            return Task.CompletedTask;
        }


        private async Task<Document> AddUnderScoreSymbolAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);

            var variableDeclarator = root.FindNode(diagnostic.Location.SourceSpan) as VariableDeclaratorSyntax;
            if (variableDeclarator == null) return document;

            //Es necesario vincular el Variable con el Field para que los valores no sean null al ser asignados.
            var fieldDeclaration = variableDeclarator.Ancestors().OfType<FieldDeclarationSyntax>().FirstOrDefault();
            if (fieldDeclaration == null) return document;

            Document newDocument = BuildNewRoot(document, root, variableDeclarator, fieldDeclaration);
            return newDocument;
        }

        private static Document BuildNewRoot(Document document, SyntaxNode root, VariableDeclaratorSyntax variableDeclarator, FieldDeclarationSyntax fieldDeclaration)
        {
            var newFieldDeclaration = ReplaceVariableName(variableDeclarator, fieldDeclaration);
            var newRoot = root.ReplaceNode(fieldDeclaration, newFieldDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private static FieldDeclarationSyntax ReplaceVariableName(VariableDeclaratorSyntax variableDeclarator, FieldDeclarationSyntax fieldDeclaration)
        {
            // Crea una lista de variables con el nombre de la variable específica y solo actualiza la variable seleccionada pero no las demás, esto por si están en una misma línea.
            var variables = fieldDeclaration.Declaration.Variables;
            var newVariables = variables.Select(v =>
            {
                if (v.Identifier.Text == variableDeclarator.Identifier.Text)
                    return v.WithIdentifier(SyntaxFactory.Identifier("s_" + v.Identifier.Text));
                return v;
            });
            return fieldDeclaration.WithDeclaration(fieldDeclaration.Declaration.WithVariables(SyntaxFactory.SeparatedList(newVariables)));
        }
    }
}
