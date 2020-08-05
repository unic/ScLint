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

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: NullStringsAnalyzer.Title,
                    createChangedDocument: c => ModifyExpression(context.Document, root, nodeToChange),
                    equivalenceKey: NullStringsAnalyzer.Title),
                diagnostic);
        }

        private async Task<Document> ModifyExpression(Document document, SyntaxNode root, SyntaxNode nodeToChange)
        {
            var investigatedToken = nodeToChange.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();

            var semanticModel = document.GetSemanticModelAsync().Result;

            string typeName = semanticModel.GetTypeInfo(investigatedToken).Type.Name;

            string newTokenText;

            if (typeName == typeof(String).Name.ToString())
            {
                newTokenText = $"string.IsNullOrEmpty({nodeToChange.DescendantTokens().FirstOrDefault()})";    
            }
            else
            {
                newTokenText = $"{nodeToChange.DescendantTokens().FirstOrDefault()} is null";
            }

            SyntaxToken newToken = SyntaxFactory.Literal(default(SyntaxTriviaList), newTokenText, newTokenText, default(SyntaxTriviaList));
            SyntaxNode newNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, newToken);
            SyntaxNode newRoot = root.ReplaceNode(nodeToChange, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}