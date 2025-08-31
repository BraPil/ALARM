using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ALARM.Indexer;

public class SymbolWalker : CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;
    private readonly CodeIndex _codeIndex;
    private readonly string _filePath;

    public SymbolWalker(SemanticModel semanticModel, CodeIndex codeIndex, string filePath)
    {
        _semanticModel = semanticModel;
        _codeIndex = codeIndex;
        _filePath = filePath;
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var symbol = _semanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            _codeIndex.Symbols.Add(CreateSymbolInfo(symbol, node, "Class"));
        }
        base.VisitClassDeclaration(node);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        var symbol = _semanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            _codeIndex.Symbols.Add(CreateSymbolInfo(symbol, node, "Interface"));
        }
        base.VisitInterfaceDeclaration(node);
    }

    public override void VisitStructDeclaration(StructDeclarationSyntax node)
    {
        var symbol = _semanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            _codeIndex.Symbols.Add(CreateSymbolInfo(symbol, node, "Struct"));
        }
        base.VisitStructDeclaration(node);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        var symbol = _semanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            _codeIndex.Symbols.Add(CreateSymbolInfo(symbol, node, "Enum"));
        }
        base.VisitEnumDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var symbol = _semanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            var symbolInfo = CreateSymbolInfo(symbol, node, "Method");
            
            // Calculate cyclomatic complexity
            symbolInfo.CyclomaticComplexity = CalculateCyclomaticComplexity(node);
            
            // Extract parameters
            symbolInfo.Parameters = node.ParameterList.Parameters
                .Select(p => $"{p.Type} {p.Identifier}")
                .ToList();

            // Extract return type
            symbolInfo.ReturnType = node.ReturnType.ToString();

            _codeIndex.Symbols.Add(symbolInfo);
        }
        base.VisitMethodDeclaration(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var symbol = _semanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            var symbolInfo = CreateSymbolInfo(symbol, node, "Property");
            symbolInfo.ReturnType = node.Type.ToString();
            _codeIndex.Symbols.Add(symbolInfo);
        }
        base.VisitPropertyDeclaration(node);
    }

    public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        foreach (var variable in node.Declaration.Variables)
        {
            var symbol = _semanticModel.GetDeclaredSymbol(variable);
            if (symbol != null)
            {
                var symbolInfo = CreateSymbolInfo(symbol, node, "Field");
                symbolInfo.ReturnType = node.Declaration.Type.ToString();
                _codeIndex.Symbols.Add(symbolInfo);
            }
        }
        base.VisitFieldDeclaration(node);
    }

    public override void VisitEventDeclaration(EventDeclarationSyntax node)
    {
        var symbol = _semanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            var symbolInfo = CreateSymbolInfo(symbol, node, "Event");
            symbolInfo.ReturnType = node.Type.ToString();
            _codeIndex.Symbols.Add(symbolInfo);
        }
        base.VisitEventDeclaration(node);
    }

    public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        var symbol = _semanticModel.GetDeclaredSymbol(node);
        if (symbol != null)
        {
            var symbolInfo = CreateSymbolInfo(symbol, node, "Constructor");
            symbolInfo.Parameters = node.ParameterList.Parameters
                .Select(p => $"{p.Type} {p.Identifier}")
                .ToList();
            _codeIndex.Symbols.Add(symbolInfo);
        }
        base.VisitConstructorDeclaration(node);
    }

    private SymbolInfo CreateSymbolInfo(ISymbol symbol, SyntaxNode node, string kind)
    {
        var lineSpan = node.GetLocation().GetLineSpan();
        
        return new SymbolInfo
        {
            Name = symbol.Name,
            FullName = symbol.ToDisplayString(),
            Kind = kind,
            FilePath = _filePath,
            LineNumber = lineSpan.StartLinePosition.Line + 1,
            Accessibility = symbol.DeclaredAccessibility.ToString(),
            IsStatic = symbol.IsStatic,
            IsAbstract = symbol.IsAbstract,
            Namespace = symbol.ContainingNamespace?.ToDisplayString() ?? "",
            ContainingType = symbol.ContainingType?.Name ?? "",
            Attributes = symbol.GetAttributes()
                .Select(attr => attr.AttributeClass?.Name ?? "")
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList()
        };
    }

    private int CalculateCyclomaticComplexity(MethodDeclarationSyntax method)
    {
        var complexity = 1; // Base complexity

        var complexityNodes = method.DescendantNodes().Where(node =>
            node.IsKind(SyntaxKind.IfStatement) ||
            node.IsKind(SyntaxKind.ElseClause) ||
            node.IsKind(SyntaxKind.WhileStatement) ||
            node.IsKind(SyntaxKind.ForStatement) ||
            node.IsKind(SyntaxKind.ForEachStatement) ||
            node.IsKind(SyntaxKind.DoStatement) ||
            node.IsKind(SyntaxKind.SwitchStatement) ||
            node.IsKind(SyntaxKind.CaseSwitchLabel) ||
            node.IsKind(SyntaxKind.CatchClause) ||
            node.IsKind(SyntaxKind.ConditionalExpression) ||
            node.IsKind(SyntaxKind.LogicalAndExpression) ||
            node.IsKind(SyntaxKind.LogicalOrExpression));

        complexity += complexityNodes.Count();

        return complexity;
    }
}
