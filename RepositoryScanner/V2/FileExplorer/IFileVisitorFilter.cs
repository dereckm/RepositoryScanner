namespace RepositoryScanner.V2.FileExplorer
{
    public interface IFileVisitorFilter
    {
        bool IsSolution(string fileExtension);
        bool IsProject(string fileExtension);
        bool IsSourceFile(string fileExtension);
    }
}