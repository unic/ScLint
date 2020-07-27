using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace MagicVariables
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MagicVariablesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint9";
        public static string Title = "This might be a magic string. Consider assigning this value to variable";
        private const string Category = "Declaration of magic variable";

        private static SyntaxKind[] investigatedNodes = { SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression, SyntaxKind.GreaterThanExpression,
                                                            SyntaxKind.GreaterThanOrEqualExpression, SyntaxKind.LessThanExpression, SyntaxKind.LessThanOrEqualExpression,
                                                            SyntaxKind.InvocationExpression};

        private static SyntaxKind[] investigatedTokens = { SyntaxKind.StringLiteralToken, SyntaxKind.NumericLiteralToken, SyntaxKind.CharacterLiteralToken };

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, investigatedNodes);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            List<SyntaxToken> childTokensList = new List<SyntaxToken>();
            foreach (SyntaxKind token in investigatedTokens)
            {
                childTokensList.Add(context.Node.DescendantTokens().FirstOrDefault(x => x.IsKind(token)));
            }
            
            if (context.Node.DescendantTokens().Any())
            {
                foreach (var childToken in childTokensList)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, childToken.GetLocation()));
                }
            }
        }
    }
}
