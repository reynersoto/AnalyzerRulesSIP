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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CommentsCapitalCodeFixProvider)), Shared]
    public class CommentsCapitalCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CommentsCapital.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
{
    foreach (var diagnostic in context.Diagnostics.Where(d => FixableDiagnosticIds.Contains(d.Id)))
    {
        context.RegisterCodeFix(CodeAction.Create(CodeFixResources.GP003TituloFix, 
                token => MakeFirstLetterOfCommentUpperCaseAsync(context.Document, diagnostic, token)), diagnostic);
    }

    return Task.FromResult<object>(null);
}


        private async Task<Document> MakeFirstLetterOfCommentUpperCaseAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var (root, node, newTrivia) = await FindTriviaAndPrepareNewOne(document, diagnostic, cancellationToken);

            var newRoot = EnlistNewTriviaAndReplaceItNewRoot(node, root, newTrivia);

            // Return document with transformed tree.
            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode EnlistNewTriviaAndReplaceItNewRoot(SyntaxTrivia node, SyntaxNode root, SyntaxTrivia newTrivia)
        {
            var listTrivias = new List<SyntaxTrivia>();
            listTrivias.Add(node);

            // Find the node that contains the trivia
            var newRoot = root.ReplaceTrivia(listTrivias, (oldTrivia, _) => newTrivia);
            return newRoot;
        }

        private static async Task<(SyntaxNode root, SyntaxTrivia node, SyntaxTrivia newTrivia)> FindTriviaAndPrepareNewOne(Document document, Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var node = root.FindTrivia(diagnostic.Location.SourceSpan.Start, true);
            
            var newTrivia = PrepareNewTriviaUsingReWriter(node);

            return (root, node, newTrivia);
        }

        private static SyntaxTrivia PrepareNewTriviaUsingReWriter(SyntaxTrivia node)
        {
            var myRewriter = new MyRewriter();
            var newTrivia = myRewriter.VisitTrivia(node);
            return newTrivia;
        }
    }
}
