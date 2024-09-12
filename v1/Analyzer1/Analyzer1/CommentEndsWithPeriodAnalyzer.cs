using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Analyzer1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentEndsWithPeriodAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GP006";

        private static readonly LocalizableString s_Title
            = new LocalizableResourceString(nameof(Resources.GP006Titulo), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_MessageFormat
            = new LocalizableResourceString(nameof(Resources.GP006Mensaje), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_Description
            = new LocalizableResourceString(nameof(Resources.GP006Descripcion), Resources.ResourceManager, typeof(Resources));
        private const string s_Category = "SIP.COMENTARIOS";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, s_Title, s_MessageFormat, s_Category,
            DiagnosticSeverity.Error, isEnabledByDefault: true, description: s_Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);

            // Encontrar todos los comentarios en el árbol de sintaxis.
            var trivia = root.DescendantTrivia().Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia)
                                                         || t.IsKind(SyntaxKind.MultiLineCommentTrivia));

            foreach (var comment in trivia)
            {
                var commentText = comment.ToString().TrimStart('/', '*').Trim().TrimEnd('*','/');

                // Verificar si el comentario tiene al menos un carácter y si no termina con un punto.
                if (!string.IsNullOrEmpty(commentText) && !commentText.EndsWith("."))
                {
                    var diagnostic = Diagnostic.Create(Rule, comment.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}