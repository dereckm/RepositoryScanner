namespace RepositoryScanner.V2.FileExplorer
{
    public class DefaultFileVisitorFilter : IFileVisitorFilter
    {
        public bool IsSolution(string fileExtension)
        {
            return fileExtension == ".sln";
        }

        public bool IsProject(string fileExtension)
        {
            return fileExtension == ".csproj";
        }

        public bool IsSourceFile(string fileExtension)
        {
            return fileExtension == ".cs";
        }
    }
}