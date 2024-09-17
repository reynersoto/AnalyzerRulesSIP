using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Analyzer1
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertySpacingCodeFixProvider)), Shared]
    public class PropertySpacingCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(PropertySpacingAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.GP004Fix,
                    createChangedDocument: c => AddBlankLinesAsync(context.Document, diagnostic, c),
                    equivalenceKey: nameof(CodeFixResources.GP004Fix)),
                diagnostic);

            return Task.CompletedTask;
        }

        private async Task<Document> AddBlankLinesAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var node = root.FindNode(diagnostic.Location.SourceSpan) as PropertyDeclarationSyntax;

            if (node != null)
            {
                // Obtener el siguiente token después de la propiedad
                var nextToken = node.GetLastToken().GetNextToken();

                // Crear los nuevos trivia con una línea en blanco
                var newTrivia = SyntaxFactory.TriviaList(
                    SyntaxFactory.CarriageReturnLineFeed
                );

                // Reemplazar el trivia existente por el nuevo trivia
                var newRoot = root.ReplaceToken(nextToken, nextToken.WithLeadingTrivia(newTrivia));
                return document.WithSyntaxRoot(newRoot);
            }

            return document;
        }
    }
}

