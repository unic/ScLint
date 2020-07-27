using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace MagicVariables
{
    public enum InvestigatedTokens
    {
        String = SyntaxKind.StringLiteralToken,
        Numeric = SyntaxKind.NumericLiteralToken,
        Character = SyntaxKind.CharacterLiteralToken
    }

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MagicVariablesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint9";
        public static string Title = "This might be a magic string. Consider assigning this value to variable";
        private const string Category = "Declaration of magic variable";

        private readonly SyntaxKind[] investigatedNodes = { SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression };

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, investigatedNodes);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var childTokens = context.Node.DescendantTokens().Where(x => x.IsKind(SyntaxKind.StringLiteralToken) || x.IsKind(SyntaxKind.NumericLiteralToken) || x.IsKind(SyntaxKind.CharacterLiteralToken));
            if (context.Node.DescendantTokens().Any())
            {
                foreach (var childToken in childTokens)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, childToken.GetLocation()));
                }
            }
        }
    }
}
