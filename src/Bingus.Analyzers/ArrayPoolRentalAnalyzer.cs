using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Bingus.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[UsedImplicitly]
public class ArrayPoolRentalAnalyzer : DiagnosticAnalyzer
{
    private const string ID = "Bingus0001";
    private const string TITLE = "Prefer calling RentDisposable over Rent.";
    private const string MESSAGE_FORMAT = "Prefer calling RentDisposable over Rent.";
    private const string DESCRIPTION = "Prefer calling RentDisposable over Rent.";
    private const string CATEGORY = "MethodCall";

    private static readonly DiagnosticDescriptor Rule =
        new (ID, TITLE, MESSAGE_FORMAT, CATEGORY, DiagnosticSeverity.Warning, true, DESCRIPTION);
    
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is MemberAccessExpressionSyntax e && e.Name.Identifier.Text.Equals("Rent"))
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Rule,
                    e.GetLocation()));
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
}