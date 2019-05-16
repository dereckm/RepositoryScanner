using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public interface IRepositoryRegistry
    {
        Repository Current { get; }
        bool TryGetNext(out Repository repository);
        Repository GetRepositoryFromPath(string path);
    }
}