using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryScanner.V2.Analysis;

namespace RepositoryScanner.V2.Structure
{
    public class ProblemFinder
    {
        private readonly List<IAnalyzer> _analyzers = new List<IAnalyzer>();
        private readonly CodeBase _codeBase;

        public ProblemFinder(CodeBase codeBase)
        {
            _codeBase = codeBase;
        }

        public void AddAnalyzer(IAnalyzer analyzer)
        {
            _analyzers.Add(analyzer);
        }

        public IEnumerable<Problem> Execute()
        {
            foreach (var analyzer in _analyzers)
            {
                foreach (var problem in analyzer.FindProblems(_codeBase))
                {
                    yield return problem;
                }
            }
        }
    }
}
