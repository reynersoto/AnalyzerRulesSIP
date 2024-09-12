using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class PropertySpacingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GP004";

        private static readonly LocalizableString s_Title
            = new LocalizableResourceString(nameof(Resources.GP004Titulo), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_MessageFormat
            = new LocalizableResourceString(nameof(Resources.GP004Mensaje), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_Description
            = new LocalizableResourceString(nameof(Resources.GP004Descripcion), Resources.ResourceManager, typeof(Resources));
        private const string s_Category = "SIP.FORMATO";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, s_Title, s_MessageFormat, s_Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: s_Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.PropertyDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            // Verifica si la propiedad contiene { get; set; }
            if (propertyDeclaration.AccessorList != null)
            {
                var accessors = propertyDeclaration.AccessorList.Accessors;

                bool hasGetSet = accessors.Any(a => a.Keyword.IsKind(SyntaxKind.GetKeyword)) &&
                                 accessors.Any(a => a.Keyword.IsKind(SyntaxKind.SetKeyword));

                if (hasGetSet)
                {
                    // Obtener el siguiente nodo (posible método o campo)
                    var nextToken = propertyDeclaration.GetLastToken().GetNextToken();
                    var nextNode = nextToken.Parent;

                    if (nextNode is MethodDeclarationSyntax || nextNode is PropertyDeclarationSyntax)
                    {
                        // Obtener trivia entre la propiedad y el siguiente nodo
                        var trailingTrivia = propertyDeclaration.GetTrailingTrivia();
                        var leadingTrivia = nextNode.GetLeadingTrivia();

                        // Verificar que hay al menos dos saltos de línea (una línea en blanco)
                        bool hasBlankLine = trailingTrivia.Any(t => t.IsKind(SyntaxKind.EndOfLineTrivia)) &&
                                            leadingTrivia.Any(t => t.IsKind(SyntaxKind.EndOfLineTrivia));

                        if (!hasBlankLine)
                        {
                            // Generar un diagnóstico si no hay una línea en blanco
                            var diagnostic = Diagnostic.Create(Rule, propertyDeclaration.GetLocation());
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }
    }
}

