using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using RepositoryReaders.Directory;
using RepositoryReaders.Path;
using RepositoryReaders.Text;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.StructureParsing.Parsers.Solutions
{
    public class SolutionParser : IParser<Solution>
    {
        private readonly IFileReader _fileReader;
        private readonly IDirectoryReader _directoryReader;
        private readonly IPathReader _pathReader;
        private const string PROJECT_INCLUDE_REGEX = "Project\\(\"{\\S+}\"\\) = \"\\S+\", \"\\S+\"";

        public SolutionParser(IFileReader fileReader, IDirectoryReader directoryReader, IPathReader pathReader)
        {
            _fileReader = fileReader;
            _directoryReader = directoryReader;
            _pathReader = pathReader;
        }

        private const string FOLDER_UP_ONE_LEVEL = "..\\";

        public Solution Parse(string path)
        {
            var projects = new List<Project>();

            var lines = _fileReader.ReadAllLines(path);

          
            foreach (var line in lines)
            {
                var baseDirectory = _pathReader.GetDirectoryName(path);

                var match = Regex.Match(line, PROJECT_INCLUDE_REGEX);
                if (match.Success)
                {

                    var result = match.Value.Split('"')[5];

                    while (result.Contains(FOLDER_UP_ONE_LEVEL))
                    {
                        var index = result.IndexOf(FOLDER_UP_ONE_LEVEL, StringComparison.Ordinal);
                        result = result.Remove(index, FOLDER_UP_ONE_LEVEL.Length);
                        baseDirectory = _directoryReader.GetParent(baseDirectory);
                    }

                    Debug.Assert(baseDirectory != null, nameof(baseDirectory) + " != null");

                    result = _pathReader.Combine(baseDirectory, result);

                    projects.Add(new Project(result, null));
                }
            }

            var solution = new Solution(path) { Projects = projects };

            return solution;
        }
    }
}
