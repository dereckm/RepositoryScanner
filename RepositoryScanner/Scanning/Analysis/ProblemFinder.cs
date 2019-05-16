using System.Collections.Generic;
using System.Linq;
using RepositoryScanner.Scanning.Analysis.Analyzers;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis
{
    public class ProblemFinder : IAnalyzer<CodeBase>
    {
        private readonly IConfigurableAnalyzerCollection _analyzers;

        public ProblemFinder(IConfigurableAnalyzerCollection analyzers)
        {
            _analyzers = analyzers;
        }

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            return _analyzers.SelectMany(analyzer => analyzer.FindProblems(codeBase));
        }
    }
}
