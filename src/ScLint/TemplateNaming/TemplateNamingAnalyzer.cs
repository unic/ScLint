using System.Collections.Immutable;
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
        public static string Title = "Change to \"TemplateId\"";
        private const string MessageFormat = "Template variable name is ambiguous";
        private const string Category = "Make template naming more explicit";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.VariableDeclarator);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var nodeText = context.Node.GetText().ToString().ToLower();

            Regex regEx = new Regex("templatename");

            if (regEx.Match(nodeText).Success)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}