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
using System.Runtime.InteropServices;
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

            bool negation = nodeToChange.DescendantTokens().Any(x => x.IsKind(SyntaxKind.ExclamationEqualsToken));

            var semanticModel = context.Document.GetSemanticModelAsync().Result;

            var investigatedToken = nodeToChange.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            string typeName = $"{semanticModel.GetTypeInfo(investigatedToken).Type.ContainingNamespace.Name}.{semanticModel.GetTypeInfo(investigatedToken).Type.Name}";

            var typeNameObject = Type.GetType(typeName);

            if (typeName == typeof(String).FullName)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: NullStringsAnalyzer.TitleStringEmptyCase,
                        createChangedDocument: c => ModifyExpression(context.Document, root, nodeToChange, negation, "stringEmpty"),
                        equivalenceKey: NullStringsAnalyzer.TitleStringEmptyCase),
                    diagnostic);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: NullStringsAnalyzer.TitleStringWhiteSpaceCase,
                        createChangedDocument: c => ModifyExpression(context.Document, root, nodeToChange, negation, "stringWhitespace"),
                        equivalenceKey: NullStringsAnalyzer.TitleStringWhiteSpaceCase),
                    diagnostic);
            }
            else
            {
                context.RegisterCodeFix(
                CodeAction.Create(
                    title: NullStringsAnalyzer.TitleObjectCase,
                    createChangedDocument: c => ModifyExpression(context.Document, root, nodeToChange, negation),
                    equivalenceKey: NullStringsAnalyzer.TitleObjectCase),
                diagnostic);
            }
        }

        private async Task<Document> ModifyExpression(Document document, SyntaxNode root, SyntaxNode nodeToChange, bool negation, [Optional] string type)
        {
            string newTokenText;

            switch (type)
            {
                case "stringEmpty":
                    newTokenText = negation ? $"!string.IsNullOrEmpty({nodeToChange.DescendantTokens().FirstOrDefault()})" : $"string.IsNullOrEmpty({nodeToChange.DescendantTokens().FirstOrDefault()})";
                    break;
                case "stringWhitespace":
                    newTokenText = negation ? $"!string.IsNullOrWhiteSpace({nodeToChange.DescendantTokens().FirstOrDefault()})" : $"string.IsNullOrWhiteSpace({nodeToChange.DescendantTokens().FirstOrDefault()})";
                    break;
                default:
                    newTokenText = negation ? $"!({nodeToChange.DescendantTokens().FirstOrDefault()} is null)" : $"{nodeToChange.DescendantTokens().FirstOrDefault()} is null";
                    break;
            } 
            
            SyntaxToken newToken = SyntaxFactory.Literal(default(SyntaxTriviaList), newTokenText, newTokenText, default(SyntaxTriviaList));
            SyntaxNode newNode = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, newToken);
            SyntaxNode newRoot = root.ReplaceNode(nodeToChange, newNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}