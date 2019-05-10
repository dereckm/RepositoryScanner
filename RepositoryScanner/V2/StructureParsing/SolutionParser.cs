using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RepositoryScanner.V2.Structure;
using Serilog;

namespace RepositoryScanner.V2.StructureParsing
{
    public class SolutionParser : IParser<Solution>
    {
        private const string PROJECT_INCLUDE_REGEX = "Project\\(\"{\\S+}\"\\) = \"\\S+\", \"\\S+\"";

        public SolutionParser()
        {
        }

        private const string FOLDER_UP_ONE_LEVEL = "..\\";

        public Solution Parse(string path)
        {
            var projects = new List<Project>();

            var lines = File.ReadAllLines(path);

          
            foreach (var line in lines)
            {
                var baseDirectory = Path.GetDirectoryName(path);

                var match = Regex.Match(line, PROJECT_INCLUDE_REGEX);
                if (match.Success)
                {

                    var result = match.Value.Split('"')[5];

                    while (result.Contains(FOLDER_UP_ONE_LEVEL))
                    {
                        var index = result.IndexOf(FOLDER_UP_ONE_LEVEL, StringComparison.Ordinal);
                        result = result.Remove(index, FOLDER_UP_ONE_LEVEL.Length);
                        baseDirectory = Directory.GetParent(baseDirectory).FullName;
                    }

                    Debug.Assert(baseDirectory != null, nameof(baseDirectory) + " != null");

                    result = Path.Combine(baseDirectory, result);

                    projects.Add(new Project(result, null));
                }
            }

            var solution = new Solution(path) { Projects = projects };

            return solution;
        }
    }
}
