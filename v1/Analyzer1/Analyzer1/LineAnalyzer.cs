using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Text;
using System.Data;

namespace Analyzer1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LineAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GP001";
        private const Int32 _maxLineLength = 170;
        // SE ESTABLECEN LAS CARACTERISTICAS DE LA REGLA ANTERIOR
        private static readonly LocalizableString s_Title 
            = new LocalizableResourceString(nameof(Resources.GP001Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_MessageFormat 
            = new LocalizableResourceString(nameof(Resources.GP001MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_Description 
            = new LocalizableResourceString(nameof(Resources.GP001Description), Resources.ResourceManager, typeof(Resources));
        private const string s_Category = "SIP.FORMATO";

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
            var text = context.Tree.GetText(context.CancellationToken);
            IsLineLenghtGreaterThan130(context, text);
        }

        private static void IsLineLenghtGreaterThan130(SyntaxTreeAnalysisContext context, SourceText text)
        {
            // ANALIZA EL DOCUMENTO Y SE ASEGURA DE QUE CADA LINEA DEL DOCUMENTO NO PASE DE 170 CARACTERES.
            foreach (var line in text.Lines)
            {
                if (line.ToString().Length > _maxLineLength)
                {
                    var diagnostic = Diagnostic.Create(Rule, Location.Create(context.Tree, line.Span));
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
