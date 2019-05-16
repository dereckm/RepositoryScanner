using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FileScanner.Scanning.StructureParsing.Parsers
{
    public class FileVisitor : CSharpSyntaxVisitor, IFileVisitor
    {
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var attributes = node.AttributeLists.SelectMany(x => x.Attributes);
            var isSerializable = attributes.Any(x => x.Name.ToString() == "Serializable");

            base.VisitClassDeclaration(node);
        }
    }
}
