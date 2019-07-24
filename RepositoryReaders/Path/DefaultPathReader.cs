using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryReaders.Path
{
    public class DefaultPathReader : IPathReader
    {
        public string GetFileNameWithoutExtension(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string GetExtension(string path)
        {
            return System.IO.Path.GetExtension(path);
        }

        public string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        public string GetDirectoryName(string path)
        {
            return System.IO.Path.GetDirectoryName(path);
        }

        public string GetFullPath(string filePath, string baseDirectory)
        {
            return System.IO.Path.GetFullPath(filePath, baseDirectory);
        }

        public string Combine(params string[] paths)
        {
            return System.IO.Path.Combine(paths);
        }
    }
}
