using System.Collections.Generic;

namespace RepositoryScanner.Scanning.Structure
{
    public class CodeBase
    {
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Solution> Solutions { get; set; } = new List<Solution>();
        public List<Repository> Repositories { get; set; } = new List<Repository>();
        public List<SourceFile> SourceFiles { get; set; } = new List<SourceFile>();
    }
}
