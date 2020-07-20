using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace QueryLimit
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class QueryLimitAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint6";
        private const string Title = "Try to limit Sitecore queries and reference items in other way";
        private const string Category = "Limit Sitecore queries";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.StringLiteralExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var nodeText = context.Node.GetText().ToString().ToLower();

            Regex regEx = new Regex(@"query:/");

            if (regEx.Match(nodeText).Success)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}
