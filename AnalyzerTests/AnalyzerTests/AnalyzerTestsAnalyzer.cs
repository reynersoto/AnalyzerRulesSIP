using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace AnalyzerTests
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AnalyzerTestsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GP001";

        // SE ESTABLECEN LAS CARACTERISTICAS DE LA REGLA ANTERIOR
        private static readonly LocalizableString s_Title = new LocalizableResourceString(nameof(Resources.GP001Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_MessageFormat = new LocalizableResourceString(nameof(Resources.GP001MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_Description = new LocalizableResourceString(nameof(Resources.GP001Description), Resources.ResourceManager, typeof(Resources));
        private const string s_Category = "SIP.Estilo";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, s_Title, s_MessageFormat, s_Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: s_Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

           
            // SE REALIZA LLAMADO AL NODO USANDO EL METODO COMO PARAMETRO.
            context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            // ANALIZA EL DOCUMENTO Y SE ASEGURA DE QUE CADA LINEA DEL DOCUMENTO NO PASE DE 130 CARACTERES.
            var root = context.Tree.GetRoot(context.CancellationToken);
            var text = context.Tree.GetText(context.CancellationToken);

            foreach (var line in text.Lines)
            {
                if (line.ToString().Length > 130)
                {
                    var diagnostic = Diagnostic.Create(Rule, Location.Create(context.Tree, line.Span));
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
