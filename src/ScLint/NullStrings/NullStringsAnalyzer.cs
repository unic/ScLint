using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace NullStrings
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class NullStringsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint12";
        public const string Title = "Use different method to check whether object is null";
        public const string TitleObjectCase = "Use 'is' operator";
        public const string TitleStringEmptyCase = "Use 'IsNullOrEmpty' method";
        public const string TitleStringWhiteSpaceCase = "Use 'IsNullOrWhiteSpace' method";
        private const string Category = "Null object check";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.DescendantNodes().Any(x => x.IsKind(SyntaxKind.NullLiteralExpression)))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}
