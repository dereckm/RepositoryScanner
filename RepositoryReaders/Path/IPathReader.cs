using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryReaders.Path
{
    public interface IPathReader
    {
        string GetFileNameWithoutExtension(string path);
        string GetExtension(string path);
        string GetFileName(string path);
        string GetDirectoryName(string path);
        string GetFullPath(string filePath, string baseDirectory);
        string Combine(params string[] paths);
    }
}
