using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryScanner.V2.Structure
{
    public class Solution : CodeBaseItem
    {
        public Solution(string path) : base(path)
        {
        }

        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
