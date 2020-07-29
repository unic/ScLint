using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace HardCodedPaths
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HardCodedPathsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint3";
        private const string Title = "Getting Sitecore items with the use of Sitecore.Context.Database namespace method is an obsolete way - use different method";
        private const string Category = "Obsolete way of getting Sitecore items";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var nodeText = context.Node.GetText().ToString().ToLower();

            Regex regEx = new Regex($@"database.(getitem|getrootitem|selectitems|selectitemsusingxpath|selectsingleitem|selectsingleitemusingxpath)\(");

            if (regEx.Match(nodeText).Success)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}