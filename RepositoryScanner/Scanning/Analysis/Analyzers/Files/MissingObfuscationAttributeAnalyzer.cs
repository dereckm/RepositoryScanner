using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RepositoryScanner.Scanning.Analysis.Analyzers.Files
{
    public class MissingObfuscationAttributeAnalyzer : IAnalyzer<ClassDeclarationSyntax>
    {
        public IEnumerable<Problem> FindProblems(ClassDeclarationSyntax node)
        {
            var attributes = node.AttributeLists.SelectMany(x => x.Attributes);
            var isObfuscated = attributes.Any(x => x.Name.ToString() == "Obfuscation");

            if (!isObfuscated)
            {
                yield return new Problem(ProblemType.MissingObfuscationTagOnClass,"Missing obfuscation tag on class.",
                    $"The following class is missing it's obfuscation tag: {node.Identifier.Text}.");
            }
        }
    }
}