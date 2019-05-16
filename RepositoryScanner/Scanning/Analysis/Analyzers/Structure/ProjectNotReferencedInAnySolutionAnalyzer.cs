using System.Collections.Generic;
using System.Linq;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis.Analyzers.Structure
{
    public class ProjectNotReferencedInAnySolutionAnalyzer : IAnalyzer<CodeBase>
    {
        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var project in codeBase.Projects)
            {
                if (!codeBase.Solutions.Any(s => s.Projects.Any(p => p.Path == project.Path)))
                {
                    yield return new Problem(ProblemType.ProjectNotReferenced, "Project not referenced in a solution.",
                        $"The following project is not referenced in any solution: {project.Path}.");
                }
            }
        }
    }
}
