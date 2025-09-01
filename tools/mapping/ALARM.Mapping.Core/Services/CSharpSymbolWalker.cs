using ALARM.Mapping.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace ALARM.Mapping.Core.Services
{
    /// <summary>
    /// C# syntax tree walker for symbol extraction
    /// </summary>
    internal class CSharpSymbolWalker : CSharpSyntaxWalker
    {
        private readonly Models.FileInfo _file;
        private readonly List<CodeSymbol> _symbols;
        private readonly Stack<CodeSymbol> _containerStack = new();

        public CSharpSymbolWalker(Models.FileInfo file, List<CodeSymbol> symbols)
        {
            _file = file;
            _symbols = symbols;
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            var namespaceSymbol = new CodeSymbol
            {
                Name = node.Name.ToString(),
                FullName = node.Name.ToString(),
                Type = SymbolType.Namespace,
                Namespace = node.Name.ToString(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = AccessModifier.Public
            };

            _symbols.Add(namespaceSymbol);
            _containerStack.Push(namespaceSymbol);

            base.VisitNamespaceDeclaration(node);

            _containerStack.Pop();
        }

        public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
        {
            var namespaceSymbol = new CodeSymbol
            {
                Name = node.Name.ToString(),
                FullName = node.Name.ToString(),
                Type = SymbolType.Namespace,
                Namespace = node.Name.ToString(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = AccessModifier.Public
            };

            _symbols.Add(namespaceSymbol);
            _containerStack.Push(namespaceSymbol);

            base.VisitFileScopedNamespaceDeclaration(node);

            _containerStack.Pop();
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var classSymbol = CreateTypeSymbol(node, SymbolType.Class);
            _symbols.Add(classSymbol);
            _containerStack.Push(classSymbol);

            // Extract base types
            if (node.BaseList != null)
            {
                foreach (var baseType in node.BaseList.Types)
                {
                    classSymbol.Metadata["BaseTypes"] = classSymbol.Metadata.GetValueOrDefault("BaseTypes", new List<string>());
                    ((List<string>)classSymbol.Metadata["BaseTypes"]).Add(baseType.Type.ToString());
                }
            }

            base.VisitClassDeclaration(node);

            _containerStack.Pop();
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var interfaceSymbol = CreateTypeSymbol(node, SymbolType.Interface);
            _symbols.Add(interfaceSymbol);
            _containerStack.Push(interfaceSymbol);

            base.VisitInterfaceDeclaration(node);

            _containerStack.Pop();
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            var structSymbol = CreateTypeSymbol(node, SymbolType.Struct);
            _symbols.Add(structSymbol);
            _containerStack.Push(structSymbol);

            base.VisitStructDeclaration(node);

            _containerStack.Pop();
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            var enumSymbol = new CodeSymbol
            {
                Name = node.Identifier.ValueText,
                FullName = GetFullName(node.Identifier.ValueText),
                Type = SymbolType.Enum,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = GetAccessModifier(node.Modifiers),
                Modifiers = node.Modifiers.Select(m => m.ValueText).ToList(),
                Attributes = ExtractAttributeNames(node.AttributeLists)
            };

            _symbols.Add(enumSymbol);
            _containerStack.Push(enumSymbol);

            base.VisitEnumDeclaration(node);

            _containerStack.Pop();
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var methodSymbol = new CodeSymbol
            {
                Name = node.Identifier.ValueText,
                FullName = GetFullName(node.Identifier.ValueText),
                Type = SymbolType.Method,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = GetAccessModifier(node.Modifiers),
                Modifiers = node.Modifiers.Select(m => m.ValueText).ToList()
            };

            // Extract parameters
            foreach (var parameter in node.ParameterList.Parameters)
            {
                var paramSymbol = new CodeSymbol
                {
                    Name = parameter.Identifier.ValueText,
                    FullName = parameter.Identifier.ValueText,
                    Type = SymbolType.Field,
                    SourceFile = _file.FullPath,
                    LineNumber = parameter.GetLocation().GetLineSpan().StartLinePosition.Line + 1
                };
                
                methodSymbol.Parameters.Add(paramSymbol);
            }

            // Extract attributes
            ExtractAttributes(node.AttributeLists, methodSymbol);

            _symbols.Add(methodSymbol);

            base.VisitMethodDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var propertySymbol = new CodeSymbol
            {
                Name = node.Identifier.ValueText,
                FullName = GetFullName(node.Identifier.ValueText),
                Type = SymbolType.Property,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = GetAccessModifier(node.Modifiers),
                Modifiers = node.Modifiers.Select(m => m.ValueText).ToList()
            };

            // Extract attributes
            ExtractAttributes(node.AttributeLists, propertySymbol);

            _symbols.Add(propertySymbol);

            base.VisitPropertyDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            foreach (var variable in node.Declaration.Variables)
            {
                var fieldSymbol = new CodeSymbol
                {
                    Name = variable.Identifier.ValueText,
                    FullName = GetFullName(variable.Identifier.ValueText),
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

            base.VisitFieldDeclaration(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            var eventSymbol = new CodeSymbol
            {
                Name = node.Identifier.ValueText,
                FullName = GetFullName(node.Identifier.ValueText),
                Type = SymbolType.Event,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = GetAccessModifier(node.Modifiers),
                Modifiers = node.Modifiers.Select(m => m.ValueText).ToList()
            };

            // Extract attributes
            ExtractAttributes(node.AttributeLists, eventSymbol);

            _symbols.Add(eventSymbol);

            base.VisitEventDeclaration(node);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
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

            base.VisitDelegateDeclaration(node);
        }

        private CodeSymbol CreateTypeSymbol(TypeDeclarationSyntax node, SymbolType symbolType)
        {
            return new CodeSymbol
            {
                Name = node.Identifier.ValueText,
                FullName = GetFullName(node.Identifier.ValueText),
                Type = symbolType,
                Namespace = GetCurrentNamespace(),
                SourceFile = _file.FullPath,
                LineNumber = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                AccessModifier = GetAccessModifier(node.Modifiers),
                Modifiers = node.Modifiers.Select(m => m.ValueText).ToList(),
                Attributes = ExtractAttributeNames(node.AttributeLists)
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
                if (modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
                    return AccessModifier.ProtectedInternal;
                return AccessModifier.Protected;
            }
            if (modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
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
