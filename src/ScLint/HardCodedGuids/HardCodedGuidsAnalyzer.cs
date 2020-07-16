using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace HardCodedGuids
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HardCodedGuidsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint4";
        private const string Title = "Create a constant class, configuration file or Sitecore item to store the GUIDs of items referenced in solution code";
        private const string Category = "Reference to unknown Sitecore item";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.Argument);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            string nodeArgumentText = context.Node.GetText().Container.CurrentText.ToString();

            string extractedGuid = nodeArgumentText.Contains("\"") ? nodeArgumentText.Substring(nodeArgumentText.IndexOf("\"") + 1, nodeArgumentText.LastIndexOf("\"") - nodeArgumentText.IndexOf("\"") - 1) : string.Empty;

            if (System.Guid.TryParse(extractedGuid, out System.Guid newGuid))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}