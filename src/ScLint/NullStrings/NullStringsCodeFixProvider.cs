using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace NullStrings
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NullStringsCodeFixProvider)), Shared]
    public class NullStringsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(NullStringsAnalyzer.DiagnosticId); }
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
            SyntaxNode nodeToChange = root.FindNode(diagnosticSpan, findInsideTrivia: true);

            var semanticModel = context.Document.GetSemanticModelAsync().Result;
            var investigatedToken = nodeToChange.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            string typeName = semanticModel.GetTypeInfo(investigatedToken).Type.Name;

            if (typeName == typeof(String).Name.ToString())
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: NullStringsAnalyzer.TitleStringEmptyCase,
                        createChangedDocument: c => ModifyStringEmpty(context.Document, root, nodeToChange),
                        equivalenceKey: NullStringsAnalyzer.TitleStringEmptyCase),
                    diagnostic);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: NullStringsAnalyzer.TitleStringWhiteSpaceCase,
                        createChangedDocument: c => ModifyStringWhiteSpace(context.Document, root, nodeToChange),
                        equivalenceKey: NullStringsAnalyzer.TitleStringWhiteSpaceCase),
                    diagnostic);
            }
            else
            {
                context.RegisterCodeFix(
                CodeAction.Create(
                    title: NullStringsAnalyzer.TitleObjectCase,
                    createChangedDocument: c => ModifyExpression(context.Document, root, nodeToChange),
                    equivalenceKey: NullStringsAnalyzer.TitleObjectCase),
                diagnostic);
            }
        }

        private async Task<Document> ModifyStringEmpty(Document document, SyntaxNode root, SyntaxNode nodeToChange)
        {
            string newTokenText = $"string.IsNullOrEmpty({nodeToChange.DescendantTokens().FirstOrDefault()})";    
            
            SyntaxToken newToken = SyntaxFactory.Literal(default(SyntaxTriviaList), newTokenText, newTokenText, default(SyntaxTriviaList));
            SyntaxNode newNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, newToken);
            SyntaxNode newRoot = root.ReplaceNode(nodeToChange, newNode);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> ModifyStringWhiteSpace(Document document, SyntaxNode root, SyntaxNode nodeToChange)
        {
            string newTokenText = $"string.IsNullOrWhiteSpace({nodeToChange.DescendantTokens().FirstOrDefault()})";

            SyntaxToken newToken = SyntaxFactory.Literal(default(SyntaxTriviaList), newTokenText, newTokenText, default(SyntaxTriviaList));
            SyntaxNode newNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, newToken);
            SyntaxNode newRoot = root.ReplaceNode(nodeToChange, newNode);
            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> ModifyExpression(Document document, SyntaxNode root, SyntaxNode nodeToChange)
        {
            string newTokenText = $"{nodeToChange.DescendantTokens().FirstOrDefault()} is null";

            SyntaxToken newToken = SyntaxFactory.Literal(default(SyntaxTriviaList), newTokenText, newTokenText, default(SyntaxTriviaList));
            SyntaxNode newNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, newToken);
            SyntaxNode newRoot = root.ReplaceNode(nodeToChange, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}