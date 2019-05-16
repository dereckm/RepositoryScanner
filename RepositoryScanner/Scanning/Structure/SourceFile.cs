namespace RepositoryScanner.Scanning.Structure
{
    public class SourceFile : CodeBaseItem
    {
        public Repository Repository { get; set; }

        public SourceFile(string path) : base(path)
        {
        }
    }
}
