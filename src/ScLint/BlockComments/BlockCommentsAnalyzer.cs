using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlockComments
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentBlocksAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint2";
        public static string Title = "Remove this block comment";
        private const string MessageFormat = "Consider removing this block comment";
        private const string Category = "Block comments";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeComment);
        }

        private void AnalyzeComment(SyntaxTreeAnalysisContext context)
        {
            SyntaxNode root = context.Tree.GetRoot(context.CancellationToken);
            IEnumerable<SyntaxTrivia> commentNodes = root.DescendantTrivia().Where(node => node.IsKind(SyntaxKind.MultiLineCommentTrivia));

            if (commentNodes != null)
            {
                foreach (SyntaxTrivia node in commentNodes)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
                }
            }
        }
    }
}