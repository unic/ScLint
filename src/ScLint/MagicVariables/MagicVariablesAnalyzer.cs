using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace MagicVariables
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MagicVariablesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint9";
        public static string Title = "This might be a magic string. Consider moving this value to variable";
        private const string Category = "Declaration of magic variable";

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
