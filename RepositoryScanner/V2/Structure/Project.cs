using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryScanner.V2.Structure
{
    public class Project : CodeBaseItem
    {
        public IEnumerable<SourceFile> SourceFiles { get; }
        public Repository Repository { get; set; }

        public Project(string path, IEnumerable<SourceFile> sourceFiles) : this(path)
        {
            SourceFiles = sourceFiles;
        }


        private Project(string path) : base(path)
        {
        }
    }
}
