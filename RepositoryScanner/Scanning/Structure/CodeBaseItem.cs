namespace RepositoryScanner.Scanning.Structure
{
    public abstract class CodeBaseItem
    {
        public string Name { get; }
        public string Path { get; }

        protected CodeBaseItem(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
        }
    }
}
