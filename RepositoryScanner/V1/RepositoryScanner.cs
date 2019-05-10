using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RepositoryScanner.V1
{
    public class RepositoryScanner
    {
        #region Private members & Nested
        private static readonly List<string> ProjectExtensions = new List<string>
        {
            ".csproj",
            ".vcxproj",
            ".vcproj",
            ".proj",
            ".rptproj",
            ".njsproj"
        };

        private static readonly List<string> SourceCodeFileExtensions = new List<string>
        {
            ".c",
            ".cpp",
            ".cs",
            ".h",
            ".js"
        };
        private class Problem
        {
            public bool IsProblematic { get; set; }
            public string ProblemDescription { get; set; }
        }
        private class ProjectReportLine : Problem
        {
            public ProjectReportLine(Problem problem)
            {
                IsProblematic = problem.IsProblematic;
                ProblemDescription = problem.ProblemDescription;
            }

            public string SolutionFilePathes { get; set; }
            public string ProjectFilePath { get; set; }
            public string ProjectName { get; set; }
            public string ProjectRepository { get; set; }

            public string ToCsvLine()
            {
                return $"\"{ProjectName}\", \"{IsProblematic}\", \"{ProblemDescription}\", \"{ProjectFilePath}\", \"{ProjectRepository}\", \"{SolutionFilePathes}\"";
            }
            public static string GetCsvHeaderLine()
            {
                return $"{nameof(ProjectName)}, {nameof(IsProblematic)}, {nameof(ProblemDescription)}, {nameof(ProjectFilePath)}, {nameof(ProjectRepository)}, Referenced in solutions";
            }
        }

        private class SourceFileReportLine : Problem
        {
            public SourceFileReportLine(Problem problem)
            {
                IsProblematic = problem.IsProblematic;
                ProblemDescription = problem.ProblemDescription;
            }

            public string FileName { get; set; }
            public string FilePath { get; set; }
            public int NumberOfReferencingProjects { get; set; }
            public string ReferencingProjects { get; set; }
            public string Repository { get; set; }

            public static string GetCsvHeaderLine()
            {
                return $"{nameof(FileName)}, " +
                       $"{nameof(IsProblematic)}, " +
                       $"{nameof(ProblemDescription)}, " +
                       $"{nameof(FilePath)}, " +
                       $"{nameof(NumberOfReferencingProjects)}, " +
                       $"{nameof(ReferencingProjects)}," +
                       $"{nameof(Repository)}";
            }
            public string ToCsvLine()
            {
                return $"\"{FileName}\", \"{IsProblematic}\", \"{ProblemDescription}\", \"{FilePath}\", \"{NumberOfReferencingProjects}\", \"{ReferencingProjects}\", \"{Repository}\"";
            }
        }

        #endregion

        public static RepositoryScanResult ScanRepository(string repositoryName,
                                                          string repositoryFolderPath)
        {
            var result = new RepositoryScanResult { RepositoryName = repositoryName };

            try
            {
                result.AllSolutionFiles = Directory.GetFiles(repositoryFolderPath, "*.sln", SearchOption.AllDirectories).Select(f=>f.ToLower()).ToList();

                result.AllProjectFiles = new List<string>();
                foreach (var projectExtension in ProjectExtensions)
                {
                    result.AllProjectFiles.AddRange(Directory.GetFiles(repositoryFolderPath,
                        $"*{projectExtension}",
                        SearchOption.AllDirectories).Select(f=>f.ToLower()).ToList());
                }

                result.AllSourceFiles = new List<string>();
                foreach (var sourceCodeFileExtension in SourceCodeFileExtensions)
                {
                    result.AllSourceFiles.AddRange(Directory.GetFiles(repositoryFolderPath,
                        $"*{sourceCodeFileExtension}",
                        SearchOption.AllDirectories).Select(f=>f.ToLower()).ToList());
                }

                result.ProjectsInSolutions = AnalyzeSolutionFiles(result.AllSolutionFiles);
                result.FilesInProjects = AnalyzeProjectsFiles(result.AllProjectFiles);

                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Error = e;
            }

            return result;
        }

        public static void SaveRawResults(RepositoryScanResult commonScanResult,
                                          string outputFolderPath)
        {
            File.WriteAllLines(Path.Combine(outputFolderPath, $"Raw_results_{commonScanResult.RepositoryName}_AllProjectFiles.txt"),
                commonScanResult.AllProjectFiles);

            File.WriteAllLines(Path.Combine(outputFolderPath, $"Raw_results_{commonScanResult.RepositoryName}_AllSolutionFiles.txt"),
                commonScanResult.AllSolutionFiles);

            File.WriteAllLines(Path.Combine(outputFolderPath, $"Raw_results_{commonScanResult.RepositoryName}_AllSourceFiles.txt"),
                commonScanResult.AllSourceFiles);

            File.WriteAllText(Path.Combine(outputFolderPath, $"Raw_results_{commonScanResult.RepositoryName}_ProjectsInSolutions.csv"),
                ToCsvString(commonScanResult.ProjectsInSolutions));

            File.WriteAllText(Path.Combine(outputFolderPath, $"Raw_results_{commonScanResult.RepositoryName}_FilesInProjects.csv"),
                ToCsvString(commonScanResult.FilesInProjects));
        }

        public static void SaveSolutionResults(RepositoryScanResult[] scanResults,
                                               string outputFolderPath)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Defined in Repository, " +
                                 "Solution Name, " +
                                 "Is Problematic," +
                                 "Problem," +
                                 "# Projects, " +
                                 "Solution Path");
            

            stringBuilder.AppendLine(string.Empty);

            foreach (var scanResult in scanResults)
            {
                foreach (var solutionPath in scanResult.AllSolutionFiles)
                {
                    var problem = IsProblematicSolution(scanResults, solutionPath);

                    stringBuilder.Append($"{scanResult.RepositoryName}, " +
                                         $"{Path.GetFileName(solutionPath)}, " +
                                         $"{problem.IsProblematic}, " +
                                         $"{problem.ProblemDescription}, " +
                                         $"{scanResult.ProjectsInSolutions.FindAll(pis => pis.SolutionFilePath.Equals(solutionPath)).Count}, " +
                                         $"{solutionPath}");

                    stringBuilder.AppendLine(string.Empty);
                }
            }

            File.WriteAllText(Path.Combine(outputFolderPath, "1_SOLUTIONS.csv"),
                stringBuilder.ToString());
        }

        public static void SaveProjectsReport(RepositoryScanResult[] scanResults,
                                              string outputFolderPath)
        {
            var stringBuilder = new StringBuilder();

            //Table Headers
            stringBuilder.AppendLine(ProjectReportLine.GetCsvHeaderLine());

            foreach (var scanResult in scanResults)
            {


                foreach (var projectFile in scanResult.AllProjectFiles)
                {
                    var problem = IsProblematicProject(scanResults, projectFile);
                    var foundInSolutions = new List<ProjectInSolution>();
                    foreach (var sr in scanResults)
                    {
                        foundInSolutions.AddRange(sr.ProjectsInSolutions.FindAll(pis => pis.ProjectFilePath.Equals(projectFile)));
                    }

                    var projectReportLine = new ProjectReportLine(problem)
                    {
                        ProjectRepository = scanResult.RepositoryName,
                        ProjectFilePath = projectFile,
                        ProjectName = Path.GetFileName(projectFile),
                        SolutionFilePathes = string.Join("+", foundInSolutions.Select(x => x.SolutionFilePath))
                    };

                    stringBuilder.AppendLine(projectReportLine.ToCsvLine());
                }
            }

            File.WriteAllText(Path.Combine(outputFolderPath, "2_PROJECTS.csv"),
                stringBuilder.ToString());
        }

        public static void SaveFilesReport(RepositoryScanResult[] scanResults,
                                            string outputFolderPath)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(SourceFileReportLine.GetCsvHeaderLine());

            var allSourceFiles = new List<string>();
            foreach (var scanResult in scanResults)
            {
                allSourceFiles.AddRange(scanResult.AllSourceFiles);
            }

            foreach (var sourceFilePath in allSourceFiles)
            {
                var reportLine = IsProblematicSourceFile(scanResults, sourceFilePath);
                stringBuilder.AppendLine(reportLine.ToCsvLine());
            }

            File.WriteAllText(Path.Combine(outputFolderPath, "3_SOURCE_FILES.csv"), stringBuilder.ToString());
        }

        public static void SetFileInProjectRepositoryProperties(RepositoryScanResult[] scanResults)
        {
            var repositoryBySourceFilePath = new Dictionary<string, string>();
            foreach (var scanResult in scanResults)
            {
                foreach (var sourceFile in scanResult.AllSourceFiles)
                {
                    repositoryBySourceFilePath.Add(sourceFile.ToLower(), scanResult.RepositoryName);
                }
            }

            var repositoryByProjectFilePath = new Dictionary<string, string>();
            foreach (var scanResult in scanResults)
            {
                foreach (var projectFile in scanResult.AllProjectFiles)
                {
                    repositoryByProjectFilePath.Add(projectFile.ToLower(), scanResult.RepositoryName);
                }
            }

            foreach (var scanResult in scanResults)
            {
                foreach (var sourceFile in scanResult.FilesInProjects)
                {
                    if(!sourceFile.FileExists) continue;
                    sourceFile.FileRepository = repositoryBySourceFilePath[sourceFile.FilePath.ToLower()];
                    if (repositoryByProjectFilePath.ContainsKey(sourceFile.ProjectFilePath.ToLower()))
                    {
                        sourceFile.ProjectRepository = repositoryByProjectFilePath[sourceFile.ProjectFilePath.ToLower()];
                    }
                }
            }
        }



        #region Private methods
        private static Problem IsProblematicProject(RepositoryScanResult[] scanResults,
                                                    string projectFile)
        {
            // Project defined in repository A is problematic if one of the following true:
            // Project has no source files
            // Files referenced by project not found
            // Project references files, which are defined in repository other then A
            // Project is not referenced in any solution

            var problemDescription = new StringBuilder();
            var result = new Problem{IsProblematic = false};
            var projectFiles = new List<FileInProject>();

            foreach (var scanResult in scanResults)
            {
                projectFiles.AddRange(scanResult.FilesInProjects.FindAll(f => f.ProjectFilePath.Equals(projectFile)));
            }

            if (projectFiles.Count == 0)
            {
                result.IsProblematic = true;
                result.ProblemDescription = "Project has no referenced source files";
                return result;
            }

            var nonExistingFiles = projectFiles.FindAll(fip => !File.Exists(fip.FilePath)).Select(fip => fip.FilePath);
            if (nonExistingFiles.Any())
            {
                result.IsProblematic = true;
                problemDescription.Append($"Project references non-existing source files");//: {string.Join("+", nonExistingFiles)}");
            }

            if (projectFiles.Any(fip => !string.IsNullOrEmpty(fip.FileRepository) &&
                                        !fip.FileRepository.Equals(fip.ProjectRepository)))
            {
                result.IsProblematic = true;
                problemDescription.Append("+Some source files reside in different repository from project file");
            }

            if (scanResults.All(sr => !sr.ProjectsInSolutions.Any(pis => pis.ProjectFilePath.ToLower().Equals(projectFile.ToLower()))))
            {
                result.IsProblematic = true;
                problemDescription.Append("+Project is not referenced by any solution");
            }

            result.ProblemDescription = problemDescription.ToString();
            return result;
        }
        private static SourceFileReportLine IsProblematicSourceFile(RepositoryScanResult[] scanResults,
                                                                    string sourceFilePath)
        {
            var result = new SourceFileReportLine(new Problem { IsProblematic = false, ProblemDescription = string.Empty })
            {
                FileName = Path.GetFileName(sourceFilePath),
                FilePath = sourceFilePath,
                Repository = scanResults.First(sr => sr.AllSourceFiles.Contains(sourceFilePath)).RepositoryName,
                NumberOfReferencingProjects = scanResults.Sum(sr => sr.FilesInProjects.Count(f => f.FilePath.Equals(sourceFilePath)))
            };

            // Source file is problematic if one of the following true:
            // It is not referenced in any project
            // It is referenced in project from different repository
            // It is referenced in more than one project


            if (result.NumberOfReferencingProjects == 0)
            {
                result.IsProblematic = true;
                result.ProblemDescription = "Source file is not referenced in any project";
                return result;
            }


            var referencingProjects = new List<FileInProject>();
            foreach (var scanResult in scanResults)
            {
                referencingProjects.AddRange(scanResult.FilesInProjects.FindAll(f => f.FilePath.Equals(sourceFilePath)));
            }

            result.ReferencingProjects = string.Join("+", referencingProjects.Select(fip => fip.ProjectFilePath));


            

            if (referencingProjects.Any(fip=>fip.FileExists &&
                                             fip.FileRepository != fip.ProjectRepository))
            {
                result.IsProblematic = true;
                var temp = referencingProjects.First(fip => fip.FileRepository != fip.ProjectRepository);
                result.ProblemDescription = $"The file resides in {temp.FileRepository} repository " +
                                            $"but referenced in {temp.ProjectName} project which resides in {temp.ProjectRepository} repository";
            }

            if (result.NumberOfReferencingProjects > 1)
            {
                result.IsProblematic = true;
                if (string.IsNullOrEmpty(result.ProblemDescription))
                {
                    result.ProblemDescription = "The source file is referenced in more than one project";
                }
                else
                {
                    result.ProblemDescription += " + source file is referenced in more than one project";
                }
            }

            return result;
        }
        private static string ToCsvString(List<ProjectInSolution> projectsInSolutions)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"{nameof(ProjectInSolution.ProjectName)}," +
                                     $"{nameof(ProjectInSolution.ProjectFilePath)}," +
                                     $"{nameof(ProjectInSolution.SolutionName)}," +
                                     $"{nameof(ProjectInSolution.SolutionFilePath)}");

            foreach (var projectInSolution in projectsInSolutions)
            {
                stringBuilder.AppendLine($"\"{projectInSolution.ProjectName}\"," +
                                         $"\"{projectInSolution.ProjectFilePath}\"," +
                                         $"\"{projectInSolution.SolutionName}\"," +
                                         $"\"{projectInSolution.SolutionFilePath}\"");
            }

            return stringBuilder.ToString();
        }
        private static string ToCsvString(List<FileInProject> filesInProjects)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"{nameof(FileInProject.FileName)}," +
                                     $"{nameof(FileInProject.FilePath)}," +
                                     $"{nameof(FileInProject.ProjectName)}," +
                                     $"{nameof(FileInProject.ProjectFilePath)}");

            foreach (var fileInProject in filesInProjects)
            {
                stringBuilder.AppendLine($"\"{fileInProject.FileName}\"," +
                                         $"\"{fileInProject.FilePath}\"," +
                                         $"\"{fileInProject.ProjectName}\"," +
                                         $"\"{fileInProject.ProjectFilePath}\"");
            }

            return stringBuilder.ToString();
        }
        private static Problem IsProblematicSolution(RepositoryScanResult[] scanResults,
                                                     string solutionPath)
        {
            var problem = new Problem { IsProblematic = false, ProblemDescription = string.Empty };

            // Solution is problematic if one of the following is true:
            // Solution references some project that doesn't exist
            // Solution doesn't reference any project at all

            var solutionProjects = new List<string>();
            foreach (var scanResult in scanResults)
            {
                solutionProjects.AddRange(scanResult.ProjectsInSolutions.
                    FindAll(pis => pis.SolutionFilePath.Equals(solutionPath)).
                    Select(pis => pis.ProjectFilePath));
            }

            if (solutionProjects.Count == 0)
            {
                problem.IsProblematic = true;
                problem.ProblemDescription = "Solution seems to not reference any project at all";
            }

            var missingProjects = solutionProjects.FindAll(p => !File.Exists(p));

            if (missingProjects.Count > 0)
            {
                problem.IsProblematic = true;
                problem.ProblemDescription = "The following projects referenced by solution doesn't exist: " +
                                             $"{string.Join(" + ", missingProjects)}";
            }

            return problem;
        }
        private static List<FileInProject> AnalyzeProjectsFiles(List<string> projectFiles)
        {
            var result = new List<FileInProject>();

            foreach (var projectFile in projectFiles)
            {
                var projectLines = File.ReadAllLines(projectFile).Select(f=>f.ToLower()).ToList();
                var sourceFileReferenceLines_1 = projectLines.ToList().FindAll(l => l.Trim().StartsWith("<compile include="));

                foreach (var referenceLine in sourceFileReferenceLines_1)
                {
                    var parts = referenceLine.Split('"');
                    var projectDirectory = Path.GetDirectoryName(projectFile);
                    var sourceFilePath = Path.GetFullPath(Path.Combine(projectDirectory, parts[1]));
                    result.Add(new FileInProject
                    {
                        FilePath = sourceFilePath.ToLower(),
                        FileName = Path.GetFileName(sourceFilePath),
                        ProjectFilePath = projectFile.ToLower(),
                        ProjectName = Path.GetFileName(projectFile)
                    });
                }


                var sourceFileReferenceLines_2 = projectLines.ToList().
                    FindAll(l => l.Trim().StartsWith("relpath = ") &&
                                 SourceCodeFileExtensions.Any(sfe=>l.Trim().EndsWith($"{sfe}\"")));

                foreach (var referenceLine in sourceFileReferenceLines_2)
                {
                    var parts = referenceLine.Split('"');
                    var projectDirectory = Path.GetDirectoryName(projectFile);
                    var sourceFilePath = Path.GetFullPath(Path.Combine(projectDirectory, parts[1]));
                    result.Add(new FileInProject
                    {
                        FilePath = sourceFilePath.ToLower(),
                        FileName = Path.GetFileName(sourceFilePath),
                        ProjectFilePath = projectFile.ToLower(),
                        ProjectName = Path.GetFileName(projectFile)
                    });
                }


                var sourceFileReferenceLines_3 = projectLines.ToList().
                    FindAll(l => l.Trim().StartsWith("relativepath=\"") &&
                                 SourceCodeFileExtensions.Any(sfe=>l.Trim().EndsWith($"{sfe}\">")));

                foreach (var referenceLine in sourceFileReferenceLines_3)
                {
                    var parts = referenceLine.Split('"');
                    var projectDirectory = Path.GetDirectoryName(projectFile);
                    var sourceFilePath = Path.GetFullPath(Path.Combine(projectDirectory, parts[1]));
                    result.Add(new FileInProject
                    {
                        FilePath = sourceFilePath.ToLower(),
                        FileName = Path.GetFileName(sourceFilePath),
                        ProjectFilePath = projectFile.ToLower(),
                        ProjectName = Path.GetFileName(projectFile)
                    });
                }


                var sourceFileReferenceLines_4 = projectLines.ToList().
                    FindAll(l => l.Trim().StartsWith("<clcompile include=\"") &&
                                 SourceCodeFileExtensions.Any(sfe=>l.Trim().EndsWith($"{sfe}\">")));

                foreach (var referenceLine in sourceFileReferenceLines_4)
                {
                    var parts = referenceLine.Split('"');
                    var projectDirectory = Path.GetDirectoryName(projectFile);
                    var sourceFilePath = Path.GetFullPath(Path.Combine(projectDirectory, parts[1]));
                    result.Add(new FileInProject
                    {
                        FilePath = sourceFilePath.ToLower(),
                        FileName = Path.GetFileName(sourceFilePath),
                        ProjectFilePath = projectFile.ToLower(),
                        ProjectName = Path.GetFileName(projectFile)
                    });
                }
            }

            return result;
        }
        private static List<ProjectInSolution> AnalyzeSolutionFiles(List<string> solutionFiles)
        {
            var result = new List<ProjectInSolution>();

            foreach (var solutionFile in solutionFiles)
            {
                var solutionLines = File.ReadLines(solutionFile);
                var projectReferenceLines = solutionLines.ToList().FindAll(l => l.StartsWith("Project(\"{"));
                foreach (var projectReferenceLine in projectReferenceLines)
                {
                    var parts = projectReferenceLine.Split('"');
                    var solutionDirectory = Path.GetDirectoryName(solutionFile);

                    if (ProjectExtensions.Any(e => parts[5].Contains(e)))
                    {
                        result.Add(new ProjectInSolution
                        {
                            SolutionFilePath = solutionFile.ToLower(),
                            SolutionName = Path.GetFileName(solutionFile),
                            ProjectFilePath = Path.GetFullPath(Path.Combine(solutionDirectory, parts[5])).ToLower(),
                            ProjectName = parts[3]
                        });
                    }
                }
            }

            return result;
        }
        #endregion
    }
}