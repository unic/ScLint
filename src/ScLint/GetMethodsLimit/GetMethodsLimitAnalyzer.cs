using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace GetMethodsLimit
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class GetMethodsLimitAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint7";
        private const string Title = "Consider getting Sitecore items with the use of methods other than \"GetAncestors()\" and \"GetDescendants()\"";
        private const string Category = "Limit use of get methods API calls";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.IdentifierName);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var nodeText = context.Node.GetText().ToString().ToLower();

            Regex regEx = new Regex(@"(getancestors|getdescendants)");

            if (regEx.Match(nodeText).Success)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}
