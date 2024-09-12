using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Analyzer1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentsCapital : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GP003";

        // SE ESTABLECEN LAS CARACTERISTICAS DE LA REGLA ANTERIOR
        private static readonly LocalizableString s_Title
            = new LocalizableResourceString(nameof(Resources.GP003Titulo), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_MessageFormat
            = new LocalizableResourceString(nameof(Resources.GP003Mensaje), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_Description
            = new LocalizableResourceString(nameof(Resources.GP003Descripcion), Resources.ResourceManager, typeof(Resources));
        private const string s_Category = "SIP.COMENTARIOS";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, s_Title, s_MessageFormat, s_Category,
            DiagnosticSeverity.Error, isEnabledByDefault: true, description: s_Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

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
                var commentText = comment.ToString().TrimStart('/', '*').Trim();

                // Verificar si el comentario tiene al menos un carácter y si empieza con minúscula.
                if (!string.IsNullOrEmpty(commentText) && char.IsLower(commentText[0]))
                {
                    var diagnostic = Diagnostic.Create(Rule, comment.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
