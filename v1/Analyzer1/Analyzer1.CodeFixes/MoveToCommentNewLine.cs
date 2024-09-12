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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MoveToCommentNewLine)), Shared]
    public class MoveToCommentNewLine : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(CommentIndependecyAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics.Where(d => FixableDiagnosticIds.Contains(d.Id)))
            {
                context.RegisterCodeFix(CodeAction.Create(CodeFixResources.GP005Fix,
                        token => MoveCommentToNewLine(context.Document, diagnostic, token)), diagnostic);
            }

            return Task.FromResult<object>(null);
        }

        private async Task<Document> MoveCommentToNewLine(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var (root, node, newTrivia) = await FindTriviaAndPrepareNewOne(document, diagnostic, cancellationToken);

            var newRoot = EnlistNewTriviaAndReplaceItNewRoot(node, root, newTrivia);

            // Return document with transformed tree.
            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode EnlistNewTriviaAndReplaceItNewRoot(SyntaxTrivia node, SyntaxNode root, SyntaxTrivia newTrivia)
        {
            var syntaxTriviaList = new List<SyntaxTrivia>();
            syntaxTriviaList.Add(node);

            // Find the node that contains the trivia
            var newRoot = root.ReplaceTrivia(syntaxTriviaList, (oldTrivia, _) => newTrivia);
            return newRoot;
        }

        private static async Task<(SyntaxNode root, SyntaxTrivia node, SyntaxTrivia newTrivia)> FindTriviaAndPrepareNewOne(Document document, Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var node = root.FindTrivia(diagnostic.Location.SourceSpan.Start, true);

            var newTrivia = PrepareNewTrivia(node);

            return (root, node, newTrivia);
        }

        private static SyntaxTrivia PrepareNewTrivia(SyntaxTrivia node)
        {
            if (node.IsKind(SyntaxKind.SingleLineCommentTrivia) || node.IsKind(SyntaxKind.MultiLineCommentTrivia))
            {
                return SyntaxFactory.Comment("\n" + node.ToString());
            }
            return node;
        }


    }
}

