using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public interface IRepositoryVisitor
    {
        void Visit();
        CodeBase GetCodeBase();
    }
}