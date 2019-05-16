using System.Collections.Generic;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis.Analyzers.Structure
{
    public class FileReferencedByMultipleProjectsAnalyzer : IAnalyzer<CodeBase>
    {
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


                    yield return new Problem(ProblemType.MultipleProjectReferenceSameFile, "File reference by more than one project.", $"{kvp.Key} is referenced by multiple projects : {projects}");
                }
            }
        }
    }
}
