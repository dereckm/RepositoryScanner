using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.V2.Analysis
{
    public class FileReferencedByMultipleProjectsAnalyzer : IAnalyzer
    {
        private const string PROBLEM_NAME = "File reference by more than one project.";

        private readonly Dictionary<string, List<Project>> _fileToProjectMappingDictionary = new Dictionary<string, List<Project>>();

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            foreach (var project in codeBase.Projects)
            {
                foreach(var sourceFile in project.SourceFiles)

                if (_fileToProjectMappingDictionary.ContainsKey(sourceFile.Path))
                {
                    _fileToProjectMappingDictionary[sourceFile.Path].Add(project);
                }
                else
                {
                    _fileToProjectMappingDictionary.Add(sourceFile.Path, new List<Project>()
                    {
                        project
                    });
                }
            }

            foreach (var kvp in _fileToProjectMappingDictionary)
            {
                if (kvp.Value.Count > 1)
                {
                    var projects = "";

                    foreach (var project in kvp.Value)
                    {
                        projects += project.Path + ", ";
                    }

                    projects = projects.Length > 1 ? projects.Remove(projects.Length - 2, 2) : projects;


                    yield return new Problem(PROBLEM_NAME, $"{kvp.Key} is referenced by multiple projects : {projects}");
                }
            }
        }
    }
}
