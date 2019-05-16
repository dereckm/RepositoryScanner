using System.Collections.Generic;
using System.Linq;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis.Analyzers.Structure
{
    public class EmptyProjectAnalyzer : IAnalyzer<CodeBase>
    {
        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var project in codeBase.Projects)
            {
                if (!project.SourceFiles.Any())
                {
                    yield return new Problem(ProblemType.EmptyProject, "Project contains no source file.", $"The following project contains no source files: {project.Path}");
                }
            }
        }
    }
}
