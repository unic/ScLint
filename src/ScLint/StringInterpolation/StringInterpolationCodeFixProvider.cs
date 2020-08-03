using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringInterpolation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringInterpolationCodeFixProvider)), Shared]
    public class StringInterpolationCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(StringInterpolationAnalyzer.DiagnosticId); }
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
                    title: StringInterpolationAnalyzer.Title,
                    createChangedDocument: c => ModifyString(context.Document, root, nodeToChange),
                    equivalenceKey: StringInterpolationAnalyzer.Title),
                diagnostic);
        }

        private async Task<Document> ModifyString(Document document, SyntaxNode root, SyntaxNode nodeToChange)
        {
            SyntaxNode newNode = nodeToChange;

            IEnumerable<SyntaxNode> childNodes = newNode.DescendantNodes().Where(x => !x.IsKind(SyntaxKind.AddExpression) && !x.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression) && x.DescendantTokens().Any());

            StringBuilder newNodeText = new StringBuilder("$\"");

            foreach (SyntaxNode node in childNodes)
            {
                if (node.IsKind(SyntaxKind.StringLiteralExpression) || node.IsKind(SyntaxKind.NumericLiteralExpression) || node.IsKind(SyntaxKind.CharacterLiteralExpression))
                {
                    var token = node.DescendantTokens().FirstOrDefault();
                    newNodeText.Append(token.ToString().Replace("\"", string.Empty).Replace("'", string.Empty));
                }
                else
                {
                    newNodeText.Append($"{{{node.GetText().ToString().Replace("\"", string.Empty).Replace("'", string.Empty).Trim()}}}");
                }
            }

            newNodeText.Append("\"");

            SyntaxToken newToken = SyntaxFactory.Literal(default(SyntaxTriviaList), newNodeText.ToString(), newNodeText.ToString(), default(SyntaxTriviaList));
            newNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, newToken);

            SyntaxNode newRoot = root.ReplaceNode(nodeToChange, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}