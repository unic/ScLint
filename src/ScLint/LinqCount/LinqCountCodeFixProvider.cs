using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace LinqCount
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LinqCountCodeFixProvider)), Shared]
    public class LinqCountCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(LinqCountAnalyzer.DiagnosticId); }
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
            SyntaxNode nodeToChange = root.FindNode(diagnosticSpan, findInsideTrivia: false).Parent.Parent.Parent;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: LinqCountAnalyzer.Title,
                    createChangedDocument: c => ModifyMethodName(context.Document, root, nodeToChange),
                    equivalenceKey: LinqCountAnalyzer.Title),
                diagnostic);
        }

        private async Task<Document> ModifyMethodName(Document document, SyntaxNode root, SyntaxNode oldNode)
        {
            string oldNodeText = oldNode.GetText().ToString();
            oldNodeText = oldNodeText.Replace(oldNodeText.Substring(oldNodeText.ToLower().LastIndexOf("count")), "Any()");

            SyntaxNode newNode = SyntaxFactory.IdentifierName(oldNodeText);
            SyntaxNode newRoot = root.ReplaceNode(oldNode, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}