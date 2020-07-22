using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace TemplateNaming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TemplateNamingCodeFixProvider)), Shared]
    public class TemplateNamingCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(TemplateNamingAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;
            SyntaxToken tokenToChange = root.FindToken(diagnosticSpan.Start, findInsideTrivia: false);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: TemplateNamingAnalyzer.Title,
                    createChangedDocument: c => ModifyTemplateName(context.Document, root, tokenToChange),
                    equivalenceKey: TemplateNamingAnalyzer.Title),
                diagnostic);
        }

        private async Task<Document> ModifyTemplateName(Document document, SyntaxNode root, SyntaxToken oldVariable)
        {
            SyntaxToken newVariable = SyntaxFactory.Identifier("TemplateID");
            SyntaxNode newRoot = root.ReplaceToken(oldVariable, newVariable);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}