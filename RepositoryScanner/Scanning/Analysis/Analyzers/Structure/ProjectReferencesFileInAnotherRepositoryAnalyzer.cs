using System.Collections.Generic;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis.Analyzers.Structure
{
    public class ProjectReferencesFileInAnotherRepositoryAnalyzer : IAnalyzer<CodeBase>
    {
        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var project in codeBase.Projects)
            {
                foreach (var sourceFile in project.SourceFiles)
                {
                    if (sourceFile.Repository.Path != project.Repository.Path)
                    {
                        yield return new Problem(ProblemType.ProjectReferenceFileInDifferentRepository, "Project references a file in a separate repository.", $"The project {project.Path} references the file {sourceFile.Path} and it exists in a different repository.");
                    }
                }
            }
        }
    }
}
