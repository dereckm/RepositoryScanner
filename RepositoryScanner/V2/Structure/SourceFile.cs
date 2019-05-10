using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryScanner.V2.Structure
{
    public class SourceFile : CodeBaseItem
    {
        public Repository Repository { get; set; }

        public SourceFile(string path) : base(path)
        {
        }
    }
}
