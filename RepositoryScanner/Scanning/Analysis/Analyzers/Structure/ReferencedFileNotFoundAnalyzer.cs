using System.Collections.Generic;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis.Analyzers.Structure
{
    public class ReferencedFileNotFoundAnalyzer : IAnalyzer<CodeBase>
    {
        private readonly HashSet<string> _allFiles = new HashSet<string>();

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var sourceFile in codeBase.SourceFiles)
            {
                _allFiles.Add(sourceFile.Path);
            }

            foreach (var project in codeBase.Projects)
            {
                foreach (var sourceFile in project.SourceFiles)
                {
                    if (!_allFiles.Contains(sourceFile.Path))
                    {
                        yield return new Problem(ProblemType.ProjectReferencedFileNotFound, "Referenced file not found.", $"The referenced file in project {project.Path} could not be found : {sourceFile.Path}.");
                    }
                }
            }
        }
    }
}
