using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.V2.Analysis
{
    public class ProjectNotReferencedInAnySolutionAnalyzer : IAnalyzer
    {
        private const string PROBLEM_NAME = "Project not referenced in a solution.";

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var project in codeBase.Projects)
            {
                if (!codeBase.Solutions.Any(s => s.Projects.Any(p => p.Path == project.Path)))
                {
                    yield return new Problem(PROBLEM_NAME,
                        $"The following project is not referenced in any solution: {project.Path}.");
                }
            }
        }
    }
}
