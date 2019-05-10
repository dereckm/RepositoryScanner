using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryScanner.V2.Structure
{
    public class CodeBase
    {
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<Solution> Solutions { get; set; } = new List<Solution>();
        public List<Repository> Repositories { get; set; } = new List<Repository>();
        public List<SourceFile> SourceFiles { get; set; } = new List<SourceFile>();

        public Repository CurrentRepository => Repositories.Last();
    }
}
