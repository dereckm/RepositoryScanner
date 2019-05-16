using System.Collections.Generic;
using RepositoryScanner.Scanning.Analysis.Analyzers;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis
{
    /// <summary>
    /// This is an empty interface because Ninject will bind will not bind a concrete type to IEnumerable..
    /// </summary>
    public interface IConfigurableAnalyzerCollection : IEnumerable<IAnalyzer<CodeBase>>
    {
    }
}