using ALARM.Mapping.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace ALARM.Mapping.Core.Services
{
    /// <summary>
    /// Visual Basic syntax tree walker for symbol extraction
    /// </summary>
    internal class VBSymbolWalker : VisualBasicSyntaxWalker
    {
        private readonly Models.FileInfo _file;
        private readonly List<CodeSymbol> _symbols;
        private readonly Stack<CodeSymbol> _containerStack = new();

        public VBSymbolWalker(Models.FileInfo file, List<CodeSymbol> symbols)
        {
            _file = file;
            _symbols = symbols;
        }

        public override void VisitNamespaceBlock(NamespaceBlockSyntax node)
        {
            var namespaceSymbol = new CodeSymbol
            {
                Name = node.NamespaceStatement.Name.ToString(),
                FullName = node.NamespaceStatement.Name.ToString(),
                Type = SymbolType.Namespace,
                Namespace = node.NamespaceStatement.Name.ToString(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = AccessModifier.Public
            };

            _symbols.Add(namespaceSymbol);
            _containerStack.Push(namespaceSymbol);

            base.VisitNamespaceBlock(node);

            _containerStack.Pop();
        }

        public override void VisitClassBlock(ClassBlockSyntax node)
        {
            var classSymbol = CreateTypeSymbol(node.ClassStatement, SymbolType.Class);
            _symbols.Add(classSymbol);
            _containerStack.Push(classSymbol);

            base.VisitClassBlock(node);

            _containerStack.Pop();
        }

        public override void VisitInterfaceBlock(InterfaceBlockSyntax node)
        {
            var interfaceSymbol = CreateTypeSymbol(node.InterfaceStatement, SymbolType.Interface);
            _symbols.Add(interfaceSymbol);
            _containerStack.Push(interfaceSymbol);

            base.VisitInterfaceBlock(node);

            _containerStack.Pop();
        }

        public override void VisitStructureBlock(StructureBlockSyntax node)
        {
            var structSymbol = CreateTypeSymbol(node.StructureStatement, SymbolType.Struct);
            _symbols.Add(structSymbol);
            _containerStack.Push(structSymbol);

            base.VisitStructureBlock(node);

            _containerStack.Pop();
        }

        public override void VisitEnumBlock(EnumBlockSyntax node)
        {
            var enumSymbol = CreateTypeSymbol(node.EnumStatement, SymbolType.Enum);
            _symbols.Add(enumSymbol);
            _containerStack.Push(enumSymbol);

            base.VisitEnumBlock(node);

            _containerStack.Pop();
        }

        public override void VisitMethodBlock(MethodBlockSyntax node)
        {
            var methodSymbol = new CodeSymbol
            {
                Name = node.SubOrFunctionStatement.Identifier.ValueText,
                FullName = GetFullName(node.SubOrFunctionStatement.Identifier.ValueText),
                Type = SymbolType.Method,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = GetAccessModifier(node.SubOrFunctionStatement.Modifiers),
                Modifiers = node.SubOrFunctionStatement.Modifiers.Select(m => m.ValueText).ToList()
            };

            // Extract parameters
            if (node.SubOrFunctionStatement.ParameterList != null)
            {
                foreach (var parameter in node.SubOrFunctionStatement.ParameterList.Parameters)
                {
                    var paramSymbol = new CodeSymbol
                    {
                        Name = parameter.Identifier.Identifier.ValueText,
                        FullName = parameter.Identifier.Identifier.ValueText,
                        Type = SymbolType.Field,
                        SourceFile = _file.FullPath,
                        LineNumber = parameter.GetLocation().GetLineSpan().StartLinePosition.Line + 1
                    };
                    
                    methodSymbol.Parameters.Add(paramSymbol);
                }
            }

            // Extract attributes
            ExtractAttributes(node.SubOrFunctionStatement.AttributeLists, methodSymbol);

            _symbols.Add(methodSymbol);

            base.VisitMethodBlock(node);
        }

        public override void VisitPropertyBlock(PropertyBlockSyntax node)
        {
            var propertySymbol = new CodeSymbol
            {
                Name = node.PropertyStatement.Identifier.ValueText,
                FullName = GetFullName(node.PropertyStatement.Identifier.ValueText),
                Type = SymbolType.Property,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = GetAccessModifier(node.PropertyStatement.Modifiers),
                Modifiers = node.PropertyStatement.Modifiers.Select(m => m.ValueText).ToList()
            };

            // Extract attributes
            ExtractAttributes(node.PropertyStatement.AttributeLists, propertySymbol);

            _symbols.Add(propertySymbol);

            base.VisitPropertyBlock(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            foreach (var declarator in node.Declarators)
            {
                foreach (var name in declarator.Names)
                {
                    var fieldSymbol = new CodeSymbol
                    {
                        Name = name.Identifier.ValueText,
                        FullName = GetFullName(name.Identifier.ValueText),
                        Type = SymbolType.Field,
                        Namespace = GetCurrentNamespace(),
                        SourceFile = _file.FullPath,
                        LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                        AccessModifier = GetAccessModifier(node.Modifiers),
                        Modifiers = node.Modifiers.Select(m => m.ValueText).ToList()
                    };

                    // Extract attributes
                    ExtractAttributes(node.AttributeLists, fieldSymbol);

                    _symbols.Add(fieldSymbol);
                }
            }

            base.VisitFieldDeclaration(node);
        }

        public override void VisitEventBlock(EventBlockSyntax node)
        {
            var eventSymbol = new CodeSymbol
            {
                Name = node.EventStatement.Identifier.ValueText,
                FullName = GetFullName(node.EventStatement.Identifier.ValueText),
                Type = SymbolType.Event,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = GetAccessModifier(node.EventStatement.Modifiers),
                Modifiers = node.EventStatement.Modifiers.Select(m => m.ValueText).ToList()
            };

            // Extract attributes
            ExtractAttributes(node.EventStatement.AttributeLists, eventSymbol);

            _symbols.Add(eventSymbol);

            base.VisitEventBlock(node);
        }

        public override void VisitDelegateStatement(DelegateStatementSyntax node)
        {
            var delegateSymbol = new CodeSymbol
            {
                Name = node.Identifier.ValueText,
                FullName = GetFullName(node.Identifier.ValueText),
                Type = SymbolType.Delegate,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = GetAccessModifier(node.Modifiers),
                Modifiers = node.Modifiers.Select(m => m.ValueText).ToList()
            };

            // Extract attributes
            ExtractAttributes(node.AttributeLists, delegateSymbol);

            _symbols.Add(delegateSymbol);

            base.VisitDelegateStatement(node);
        }

        private CodeSymbol CreateTypeSymbol(DeclarationStatementSyntax node, SymbolType symbolType)
        {
            var identifier = GetIdentifierFromDeclaration(node);
            return new CodeSymbol
            {
                Name = identifier,
                FullName = GetFullName(identifier),
                Type = symbolType,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = AccessModifier.Public, // Simplified for VB
                Modifiers = new List<string>(),
                Attributes = new List<string>()
            };
        }

        private string GetIdentifierFromDeclaration(DeclarationStatementSyntax node)
        {
            return node switch
            {
                ClassStatementSyntax classStmt => classStmt.Identifier.ValueText,
                InterfaceStatementSyntax interfaceStmt => interfaceStmt.Identifier.ValueText,
                StructureStatementSyntax structStmt => structStmt.Identifier.ValueText,
                EnumStatementSyntax enumStmt => enumStmt.Identifier.ValueText,
                _ => "Unknown"
            };
        }

        private AccessModifier GetAccessModifier(SyntaxTokenList modifiers)
        {
            if (modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                return AccessModifier.Public;
            if (modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
                return AccessModifier.Private;
            if (modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            {
                if (modifiers.Any(m => m.IsKind(SyntaxKind.FriendKeyword)))
                    return AccessModifier.ProtectedInternal;
                return AccessModifier.Protected;
            }
            if (modifiers.Any(m => m.IsKind(SyntaxKind.FriendKeyword)))
                return AccessModifier.Internal;

            return AccessModifier.Internal; // Default for types, private for members
        }

        private void ExtractAttributes(SyntaxList<AttributeListSyntax> attributeLists, CodeSymbol symbol)
        {
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    symbol.Attributes.Add(attribute.Name.ToString());
                }
            }
        }

        private List<string> ExtractAttributeNames(SyntaxList<AttributeListSyntax> attributeLists)
        {
            var attributes = new List<string>();
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    attributes.Add(attribute.Name.ToString());
                }
            }
            return attributes;
        }

        private string GetCurrentNamespace()
        {
            var namespaceSymbol = _containerStack.FirstOrDefault(s => s.Type == SymbolType.Namespace);
            return namespaceSymbol?.Name ?? string.Empty;
        }

        private string GetFullName(string name)
        {
            var parts = new List<string>();
            
            var currentNamespace = GetCurrentNamespace();
            if (!string.IsNullOrEmpty(currentNamespace))
                parts.Add(currentNamespace);

            var typeSymbols = _containerStack.Where(s => s.Type == SymbolType.Class || s.Type == SymbolType.Interface || s.Type == SymbolType.Struct).Reverse();
            foreach (var typeSymbol in typeSymbols)
            {
                parts.Add(typeSymbol.Name);
            }

            parts.Add(name);
            return string.Join(".", parts);
        }
    }
}
