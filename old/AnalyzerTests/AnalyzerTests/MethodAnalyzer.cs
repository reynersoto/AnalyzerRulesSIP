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
        private const Int32 _maxMethodLength = 40;
        private Int32 lineCounter = 0;

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
        public MethodAnalyzer(){}
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(context =>
            {
                var block = GetBlock(context.Node);
                var typeOfBlock = context.Node;
                if (block is BlockSyntax && typeOfBlock is MethodDeclarationSyntax)
                {
                    var sourceTextLines = block.GetText().Lines;
                    if (IsMethodBodyLengthGreaterThanExpected(sourceTextLines, lineCounter))
                        context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
                    return;
                }
            }, SyntaxKind.IfStatement, SyntaxKind.ForEachStatement, SyntaxKind.ForStatement
                ,SyntaxKind.LocalFunctionStatement,
                 SyntaxKind.MethodDeclaration);
        }
        
        private static Boolean IsMethodBodyLengthGreaterThanExpected(TextLineCollection lineas, Int32 lineCounter)
        {
            foreach (var line in lineas)
                lineCounter++;
            lineCounter = lineCounter - 3;
            return lineCounter > _maxMethodLength ? true : false;
        }
        public static StatementSyntax GetBlock(SyntaxNode node)
            => node switch
            {
                ForEachStatementSyntax forEachStatement => forEachStatement.Statement,
                IfStatementSyntax ifStatement => ifStatement.Statement,
                ForStatementSyntax forStatement => forStatement.Statement,
                LocalFunctionStatementSyntax localFunction => localFunction.Body,
                MethodDeclarationSyntax methodDeclaration => methodDeclaration.Body,
                _ => null
            };
    }
}
