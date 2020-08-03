using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Linq;

namespace StringInterpolation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class StringInterpolationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint11";
        public const string Title = "Interpolate this string";
        private const string MessageFormat = "It is more briefly to interpolate this string";
        private const string Category = "String interpolation";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AddExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.DescendantTokens().Any(x => x.IsKind(SyntaxKind.IdentifierToken)))
            {
                if (!context.Node.Parent.IsKind(SyntaxKind.AddExpression))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
                }
            }
        }
    }
}
