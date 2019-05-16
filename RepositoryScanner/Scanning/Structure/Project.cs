using System.Collections.Generic;

namespace RepositoryScanner.Scanning.Structure
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
