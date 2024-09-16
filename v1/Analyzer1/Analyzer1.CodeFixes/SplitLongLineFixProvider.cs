using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analyzer1
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LineLengthCodeFixProvider)), Shared]
    public class LineLengthCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(LineAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.GP001Fix,
                    createChangedDocument: c => DivideLongLineAsync(context.Document, diagnostic, c),
                    equivalenceKey: nameof(CodeFixResources.GP001Fix)),
                diagnostic);

            return Task.CompletedTask;
        }


        private async Task<Document> DivideLongLineAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var lineSpan = diagnostic.Location.SourceSpan;

            var line = text.Lines.GetLineFromPosition(lineSpan.Start);
            var lineText = line.ToString();

            var newLineText = DivideLine(lineText);

            var newText = text.Replace(line.Span, newLineText);

            return document.WithText(newText);
        }

        private string DivideLine(string line)
        {
            // Busca el primer punto antes de los 170 caracteres
            var lastDotIndex = line.LastIndexOf('.', 169);

            if (lastDotIndex != -1)
            {
                // Si se encuentra un punto antes del límite, se divide en ese punto
                return line.Substring(0, lastDotIndex + 1) + "\n" + line.Substring(lastDotIndex + 1).Trim();
            }
            else
            {
                // Si no se encuentra un punto, busca la última coma antes del límite
                var lastCommaIndex = line.LastIndexOf(',', 169);

                if (lastCommaIndex != -1)
                {
                    // Si se encuentra una coma antes del límite, se divide en esa coma
                    return line.Substring(0, lastCommaIndex + 1) + "\n" + line.Substring(lastCommaIndex + 1).Trim();
                }
                else
                {
                    // Si no se encuentra ni punto ni coma, se divide en el carácter 169
                    return line.Substring(0, 169) + "\n" + line.Substring(169).Trim();
                }
            }
        }
    }
}
