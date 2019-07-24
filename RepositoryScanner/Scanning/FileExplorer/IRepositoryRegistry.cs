using System.Collections.Generic;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public interface IRepositoryRegistry : IEnumerable<Repository>
    {
        Repository Current { get; }
        Repository GetRepositoryFromPath(string path);
    }
}