using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace NullStrings
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class NullStringsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint12";
        public const string Title = "Use different method to check whether object is null";
        public const string TitleObjectCase = "Use 'is' operator";
        public const string TitleStringEmptyCase = "Use 'IsNullOrEmpty' method";
        public const string TitleStringWhiteSpaceCase = "Use 'IsNullOrWhiteSpace' method";
        private const string Category = "Null object check";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var nodeToCheck = context.Node.DescendantNodes().FirstOrDefault();
            string nodeTypeName = $"{context.SemanticModel.GetTypeInfo(nodeToCheck).Type.ContainingNamespace}.{context.SemanticModel.GetTypeInfo(nodeToCheck).Type.Name}";

            Type nodeType = Type.GetType(nodeTypeName);

            if (nodeType is null)
            {
                nodeType = Type.GetType("System.Object");
            }

            if (context.Node.DescendantNodes().Any(x => x.IsKind(SyntaxKind.NullLiteralExpression)) && (nodeType.FullName == typeof(Nullable).FullName || !nodeType.GetTypeInfo().IsValueType))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}
