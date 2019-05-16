using System.Collections.Generic;

namespace RepositoryScanner.Scanning.Structure
{
    public class Solution : CodeBaseItem
    {
        public Solution(string path) : base(path)
        {
        }

        public List<Project> Projects { get; set; } = new List<Project>();
    }
}
