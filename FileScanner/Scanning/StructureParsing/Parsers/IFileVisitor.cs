using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FileScanner.Scanning.StructureParsing.Parsers
{
    public interface IFileVisitor
    {
        void VisitClassDeclaration(ClassDeclarationSyntax node);
        void Visit(SyntaxNode node);
    }
}