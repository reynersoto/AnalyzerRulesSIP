using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace Analyzer1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentIndependecyAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GP005";
        // SE ESTABLECEN LAS CARACTERISTICAS DE LA REGLA ANTERIOR
        private static readonly LocalizableString s_Title
            = new LocalizableResourceString(nameof(Resources.GP005Titulo), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_MessageFormat
            = new LocalizableResourceString(nameof(Resources.GP005Mensaje), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_Description
            = new LocalizableResourceString(nameof(Resources.GP005Descripcion), Resources.ResourceManager, typeof(Resources));
        private const string s_Category = "SIP.Estilo";

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
            IsCommentMixedWithCode(context, trivia);
        }

        private static void IsCommentMixedWithCode(SyntaxTreeAnalysisContext context, IEnumerable<SyntaxTrivia> text)
        {
            var count = 1;
            // ANALIZA EL DOCUMENTO Y SE ASEGURA DE QUE LOS COMENTARIOS NO ESTEN MEZCLADOS CON CODIGO. DEBEN ESTAR EN LINEAS INDEPENDIENTES.
            foreach (var comment in text)
            {
                var syntaxToken = comment.Token;
                if (String.IsNullOrWhiteSpace((syntaxToken.LeadingTrivia.ToString())))
                {
                    ReportDiagnostic(context, comment, count, syntaxToken);
                }
                count =count+1;
            }
        }

        private static void ReportDiagnostic(SyntaxTreeAnalysisContext context, SyntaxTrivia comment, int count,
            SyntaxToken syntaxToken)
        {
            var diagnostic = Diagnostic.Create(Rule, Location.Create(context.Tree, comment.Span));
            context.ReportDiagnostic(diagnostic);
            Console.WriteLine("Prueba {0}: muestra: {1}, tiene: {2}", count, comment.Token.LeadingTrivia, syntaxToken);
            Console.WriteLine("*****************");
        }
    }
}