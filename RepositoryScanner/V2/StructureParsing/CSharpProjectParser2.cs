using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.V2.StructureParsing
{
    public class CSharpProjectParser2 : IParser<Project>
    {
        private const string FOLDER_UP_ONE_LEVEL = "..\\";

        private readonly Repository _repository;

        public CSharpProjectParser2(Repository repository)
        {
            _repository = repository;
        }

        public Project Parse(string path)
        {
            var reader = XmlReader.Create(path);
            var directory = Path.GetDirectoryName(path);
            var sourceFiles = new List<SourceFile>();

            while (reader.ReadToFollowing("Compile"))
            {
                var baseDirectory = directory;
                var repository = _repository;
                var filePath = reader.GetAttribute("Include");
                filePath = AdjustAbsolutePath(filePath, baseDirectory, ref repository);
                sourceFiles.Add(new SourceFile(filePath)
                {
                    Repository = repository
                });
            }

            return new Project(path, sourceFiles);
        }

        private string AdjustAbsolutePath(string result, string baseDirectory, ref Repository repository)
        {
            while (result.Contains(FOLDER_UP_ONE_LEVEL))
            {
                var index = result.IndexOf(FOLDER_UP_ONE_LEVEL, StringComparison.Ordinal);
                result = result.Remove(index, FOLDER_UP_ONE_LEVEL.Length);
                baseDirectory = Directory.GetParent(baseDirectory).FullName;

                if (baseDirectory == _repository.Path && result.Contains(FOLDER_UP_ONE_LEVEL))
                {
                    repository = new Repository(Path.Combine(Path.GetDirectoryName(_repository.Path),
                        result.Remove(0, FOLDER_UP_ONE_LEVEL.Length).Split('\\')[0]));
                }
            }

            Debug.Assert(baseDirectory != null, nameof(baseDirectory) + " != null");

            return Path.Combine(baseDirectory, result);
        }

    }
}
