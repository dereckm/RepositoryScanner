using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryReaders.Directory
{
    public interface IDirectoryReader
    {
        string GetCurrentDirectory();
        string[] GetFiles(string path, string format);
        IEnumerable<string> EnumerateFiles(string path);
        IEnumerable<string> EnumerateDirectories(string path);
        string GetParent(string path);
    }
}
