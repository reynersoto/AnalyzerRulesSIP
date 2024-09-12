using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzer1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class StaticFieldNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GP007";

        private static readonly LocalizableString s_Title
            = new LocalizableResourceString(nameof(Resources.GP007Titulo), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_MessageFormat
            = new LocalizableResourceString(nameof(Resources.GP007Mensaje), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_Description
            = new LocalizableResourceString(nameof(Resources.GP007Descripcion), Resources.ResourceManager, typeof(Resources));
        private const string s_Category = "SIP.CAMPOS";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, s_Title, s_MessageFormat, s_Category,
            DiagnosticSeverity.Error, isEnabledByDefault: true, description: s_Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        }

        private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            // Verificar si el campo es estático pero no tiene [ThreadStatic]
            bool isStatic = fieldDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
            bool isThreadStatic = fieldDeclaration.AttributeLists
                .SelectMany(attrList => attrList.Attributes)
                .Any(attr => attr.Name.ToString() == "ThreadStatic");

            if (isStatic && !isThreadStatic)
            {
                foreach (var variable in fieldDeclaration.Declaration.Variables)
                {
                    var fieldName = variable.Identifier.Text;

                    if (!fieldName.StartsWith("s_"))
                    {
                        // Campo estático pero no empieza con "s_"
                        var diagnostic = Diagnostic.Create(Rule, variable.GetLocation());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
