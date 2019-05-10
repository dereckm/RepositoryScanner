using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.V2.Analysis
{
    public interface IAnalyzer
    {
        IEnumerable<Problem> FindProblems(CodeBase codeBase);
    }
}
