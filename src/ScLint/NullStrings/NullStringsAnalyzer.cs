using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace NullStrings
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class NullStringsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint12";
        public const string Title = "Replace this expression with 'IsNullOrEmpty' or 'IsNullOrWhiteSpace' method";
        private const string MessageFormat = "Use 'IsNullOrEmpty' or 'IsNullOrWhiteSpace' method";
        private const string Category = "Null string check";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }
    }
}
