namespace RepositoryReaders.Text
{
    public interface IFileReader
    {
        string[] ReadAllLines(string path);
        bool Exists(string path);
        string ReadAllText(string path);
    }
}
