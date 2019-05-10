using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.V2.Analysis
{
    public class FileNeverReferencedAnalyzer : IAnalyzer
    {
        public const string PROBLEM_NAME = "File not referenced by any project.";

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var file in codeBase.SourceFiles)
            {
                if (!IsReferenced(file, codeBase.Projects))
                {
                    yield return new Problem(PROBLEM_NAME, $"The following file is not referenced by any projects : {file.Path}");
                }
            }
        }

        private bool IsReferenced(SourceFile sourceFile, IEnumerable<Project> projects)
        {
            return projects.Any(project => project.SourceFiles.Any(x => x.Path == sourceFile.Path));
        }
    }
}
