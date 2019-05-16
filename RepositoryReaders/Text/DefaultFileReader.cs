using System.IO;

namespace RepositoryReaders.Text
{
    public class DefaultFileReader : IFileReader
    {
        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
