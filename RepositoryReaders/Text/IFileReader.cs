namespace RepositoryReaders.Text
{
    public interface IFileReader
    {
        string[] ReadAllLines(string path);
        bool FileExists(string path);
        string ReadAllText(string path);
    }
}
