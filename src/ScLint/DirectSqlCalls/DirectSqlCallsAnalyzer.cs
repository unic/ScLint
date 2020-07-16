using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DirectSqlCalls
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DirectSqlCallsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ScLint1";
        private const string Title = "Direct SQL calls are not allowed";
        private const string Category = "Direct SQL calls";

        private static string[] sqlKeywords = { "add", "add constraint", "alter", "alter column", "alter table", "all", "and", "any", "as", "asc",
        "backup database", "between", "case", "check", "column", "constraint", "create", "create database", "create index", "create or replace view",
        "create table", "create procedure", "create unique index", "create view", "database", "default", "delete", "desc", "distinct", "drop",
        "drop column", "drop constraint", "drop database", "drop default", "drop index", "drop table", "drop view", "exec", "exists", "foreign key",
        "from", "full outer join", "group by", "having", "in", "index", "inner join", "insert into", "insert into select", "is null", "is not null",
        "join", "left join", "like", "limit", "not", "not null", "or", "order by", "outer join", "primary key", "procedure", "right join", "rownum",
        "select", "select distinct", "select into", "select top", "set", "table", "top", "truncate table", "union", "union all", "unique",
        "update", "values", "view", "where" };

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Title);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.StringLiteralExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var nodeText = context.Node.GetText().ToString().ToLower();

            foreach (var keyword in sqlKeywords)
            {
                Regex fullRegEx = new Regex($@"(select|create|insert|update|delete|drop|truncate|alter)\s+.*{keyword}\s+");
                if (fullRegEx.Match(nodeText).Success)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
                }
            }
        }
    }
}