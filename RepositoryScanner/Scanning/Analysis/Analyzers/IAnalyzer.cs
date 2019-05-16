using System.Collections.Generic;

namespace RepositoryScanner.Scanning.Analysis.Analyzers
{
    public interface IAnalyzer
    {

    }

    public interface IAnalyzer<in T> : IAnalyzer
    {
        IEnumerable<Problem> FindProblems(T analyzedObject);
    }
}
