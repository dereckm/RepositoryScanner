using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.V2.Analysis
{
    public class ReferencedFileNotFoundAnalyzer : IAnalyzer
    {
        private const string PROBLEM_NAME = "Referenced file not found.";

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var project in codeBase.Projects)
            {
                foreach (var sourceFile in project.SourceFiles)
                {
                    if (!codeBase.SourceFiles.Exists(x => x.Path == sourceFile.Path))
                    {
                        yield return new Problem(PROBLEM_NAME, $"The referenced file in project {project.Path} could not be found : {sourceFile.Path}.");
                    }
                }
            }
        }
    }
}
