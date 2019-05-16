using System.Collections.Generic;
using System.Linq;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis.Analyzers.Structure
{
    public class EmptySolutionAnalyzer : IAnalyzer<CodeBase>
    {
        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var solution in codeBase.Solutions)
            {
                if (!solution.Projects.Any())
                {
                    yield return new Problem(ProblemType.EmptySolution, "The solution should contain at least one project.", $"The following solution did not contain any projects: {solution.Path}.");
                }
            }
        }
    }
}
