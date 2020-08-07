using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace LinqCount
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LinqCountAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint10";
        public static string Title = "Change to \"Any()\"";
        private const string MessageFormat = "Use \"Any()\" method to check whether this collection is empty or not";
        private const string Category = "Use more briefly method";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.IdentifierName);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var nodeText = context.Node.GetText().ToString().ToLower();

            Regex regEx = new Regex("count");

            if (regEx.Match(nodeText).Success)
            {
                Regex parentRegEx = new Regex("count\\(\\)\\s*>\\s*0");

                SyntaxNode parentNode = context.Node.Parent.Parent.Parent;

                var parentNodeText = parentNode.GetText().ToString().ToLower();

                if (parentRegEx.Match(parentNodeText).Success)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
                }
            }
        }
    }
}