namespace RepositoryScanner.Scanning.FileExplorer
{
    public interface IRepositoryVisitorFilter
    {
        bool IsSolutionFile(string fileExtension);
        bool IsProjectFile(string fileExtension);
        bool IsSourceFile(string fileExtension);
    }
}