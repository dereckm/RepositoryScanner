using System.Collections.Generic;
using System.IO;
using RepositoryReaders.Xml;
using RepositoryScanner.Scanning.FileExplorer;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.StructureParsing.Parsers.Projects
{
    public class VCProjectParser : IParser<Project>
    {
        private readonly IXmlReader _xmlReader;
        private readonly IRepositoryRegistry _repositoryRegistry;

        public VCProjectParser(IRepositoryRegistry repositoryRegistry, IXmlReader xmlReader)
        {
            _xmlReader = xmlReader;
            _repositoryRegistry = repositoryRegistry;
        }

        public Project Parse(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var sourceFiles = new List<SourceFile>();

            _xmlReader.Create(path);

            while (_xmlReader.ReadToFollowing("File"))
            {
                var baseDirectory = directory;
                var filePath = _xmlReader.GetAttribute("RelativePath");
                filePath = Path.GetFullPath(filePath, baseDirectory);
                sourceFiles.Add(new SourceFile(filePath)
                {
                    Repository = _repositoryRegistry.GetRepositoryFromPath(filePath)
                });
            }

            return new Project(path, sourceFiles);
        }
    }
}