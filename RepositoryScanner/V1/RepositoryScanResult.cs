using System;
using System.Collections.Generic;

namespace RepositoryScanner.V1
{
    public class RepositoryScanResult
    {
        public string RepositoryName { get; set; }
        public bool IsSuccess { get; set; }
        public Exception Error { get; set; }
        public List<string> AllSolutionFiles { get; set; }
        public List<string> AllProjectFiles { get; set; }
        public List<string> AllSourceFiles { get; set; }
        public List<ProjectInSolution> ProjectsInSolutions { get; set; }
        public List<FileInProject> FilesInProjects { get; set; }
    }
}