using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace StringInterpolation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class StringInterpolationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint11";
        private const string Title = "Interpolate this string";
        private const string MessageFormat = "It is more briefly to inerpolate this string";
        private const string Category = "String interpolation";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.StringLiteralExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
           
        }
    }
}
