using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace BlockComments
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CommentBlocksCodeFixProvider)), Shared]
    public class CommentBlocksCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CommentBlocksAnalyzer.DiagnosticId); }
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
            SyntaxTrivia comment = root.FindTrivia(diagnosticSpan.Start, findInsideTrivia: true);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CommentBlocksAnalyzer.Title,
                    createChangedDocument: c => ModifyBlockComment(context.Document, root, comment),
                    equivalenceKey: CommentBlocksAnalyzer.Title),
                diagnostic);
        }

        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<Document> ModifyBlockComment(Document document, SyntaxNode root, SyntaxTrivia comment)
        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            SyntaxTrivia newComment = SyntaxFactory.Comment(string.Empty);
            SyntaxNode newRoot = root.ReplaceTrivia(comment, newComment);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}