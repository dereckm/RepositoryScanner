using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryScanner.V2.Structure
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
