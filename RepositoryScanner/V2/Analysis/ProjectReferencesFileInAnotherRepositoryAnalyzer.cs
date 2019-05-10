using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.V2.Analysis
{
    public class ProjectReferencesFileInAnotherRepositoryAnalyzer : IAnalyzer
    {
        private const string PROBLEM_NAME = "Project references a file in a separate repository.";

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var project in codeBase.Projects)
            {
                foreach (var sourceFile in project.SourceFiles)
                {
                    if (sourceFile.Repository.Path != project.Repository.Path)
                    {
                        yield return new Problem(PROBLEM_NAME, $"The project {project.Path} references the file {sourceFile.Path} and it exists in a different repository.");
                    }
                }
            }
        }
    }
}
