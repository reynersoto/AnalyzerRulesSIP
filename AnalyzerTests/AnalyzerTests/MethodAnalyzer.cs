using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Text;

namespace AnalyzerTests
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodAnalyzer : DiagnosticAnalyzer
    {
        #region VARIABLES
        public const string DiagnosticId = "GP002";
        //private const Int32 _maxMethodLength = 40;

        // SE ESTABLECEN LAS CARACTERISTICAS DE LA REGLA ANTERIOR
        private static readonly LocalizableString s_Titulo 
            = new LocalizableResourceString(nameof(Resources.GP001Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_Mensaje 
            = new LocalizableResourceString(nameof(Resources.GP002Mensaje), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_Descripcion 
            = new LocalizableResourceString(nameof(Resources.GP002Descripcion), Resources.ResourceManager, typeof(Resources));
        private const string s_Category = "SIP.Estructura";
        #endregion

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, s_Titulo, s_Mensaje, s_Category,
            DiagnosticSeverity.Error, isEnabledByDefault: true, description: s_Descripcion);

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
            IsMethodBodyLenghtGreaterThan40(context, text);
        }

        private static void IsMethodBodyLenghtGreaterThan40(SyntaxTreeAnalysisContext context, SourceText text)
        {
            // ANALIZA EL DOCUMENTO Y SE ASEGURA DE QUE CADA LINEA DEL DOCUMENTO NO PASE DE 130 CARACTERES.
            //foreach (var line in text.Lines)
            //{
            //    if (line.ToString().Length > _maxMethodLength)
            //    {
            //        var diagnostic = Diagnostic.Create(Rule, Location.Create(context.Tree, line.Span));
            //        context.ReportDiagnostic(diagnostic);
            //    }
            //}
        }
    }
}
