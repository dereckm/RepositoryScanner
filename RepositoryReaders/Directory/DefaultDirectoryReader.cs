using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RepositoryReaders.Directory
{
    public class DefaultDirectoryReader : IDirectoryReader
    {
        public string GetCurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        public string[] GetFiles(string path, string format)
        {
            return System.IO.Directory.GetFiles(path, format);
        }

        public IEnumerable<string> EnumerateFiles(string path)
        {
            return System.IO.Directory.EnumerateFiles(path);
        }

        public IEnumerable<string> EnumerateDirectories(string path)
        {
            return System.IO.Directory.EnumerateDirectories(path);
        }

        public string GetParent(string path)
        {
            return System.IO.Directory.GetParent(path).FullName;
        }
    }
}
