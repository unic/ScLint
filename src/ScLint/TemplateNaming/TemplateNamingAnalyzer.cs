using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TemplateNaming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TemplateNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint8";
        private const string PropertyMatchName = "TemplateName";
        public static string Title = $"Change to \"TemplateID\"";
        private const string Category = "Make template naming more explicit";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var nodeText = context.Node.GetText().ToString();
            Regex regEx = new Regex(PropertyMatchName);

            if (regEx.Match(nodeText).Success)
            {
                var identifierToken = context.Node.DescendantTokens().FirstOrDefault(x => x.Text.Equals(PropertyMatchName));
                context.ReportDiagnostic(Diagnostic.Create(Rule, identifierToken.GetLocation()));
            }
        }
    }
}