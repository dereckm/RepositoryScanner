using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.V2.Analysis
{
    public class EmptySolutionAnalyzer : IAnalyzer
    {
        private const string PROBLEM_NAME = "The solution should contain at least one project.";

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var solution in codeBase.Solutions)
            {
                if (!solution.Projects.Any())
                {
                    yield return new Problem(PROBLEM_NAME, $"The following solution did not contain any projects: {solution.Path}.");
                }
            }
        }
    }
}
