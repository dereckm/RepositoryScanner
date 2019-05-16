using System;

namespace RepositoryScanner.Scanning.Structure
{
    public class Repository : CodeBaseItem, ICloneable
    {
        public Repository(string path) : base(path)
        {
        }

        public object Clone()
        {
            return new Repository(this.Path);
        }
    }
}
