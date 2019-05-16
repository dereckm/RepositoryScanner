using System.Collections.Generic;
using System.Linq;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis.Analyzers.Structure
{
    public class FileNeverReferencedAnalyzer : IAnalyzer<CodeBase>
    {
        private readonly HashSet<string> _filesReferencedByProjects = new HashSet<string>();

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var project in codeBase.Projects)
            {
                _filesReferencedByProjects.UnionWith(project.SourceFiles.Select(x => x.Path));
            }

            foreach (var file in codeBase.SourceFiles)
            {
                if (!IsReferenced(file))
                {
                    yield return new Problem(ProblemType.FileNotReferenced, "File not referenced by any project.", $"The following file is not referenced by any projects : {file.Path}");
                }
            }
        }

        private bool IsReferenced(SourceFile sourceFile)
        {
            return _filesReferencedByProjects.Contains(sourceFile.Path);
        }
    }
}
