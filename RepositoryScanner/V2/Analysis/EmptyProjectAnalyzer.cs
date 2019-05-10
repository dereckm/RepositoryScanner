using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.V2.Analysis
{
    public class EmptyProjectAnalyzer : IAnalyzer
    {
        public const string PROBLEM_NAME = "Project contains no source file.";

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var project in codeBase.Projects)
            {
                if (!project.SourceFiles.Any())
                {
                    yield return new Problem(PROBLEM_NAME, $"The following project contains no source files: {project.Path}");
                }
            }
        }
    }
}
