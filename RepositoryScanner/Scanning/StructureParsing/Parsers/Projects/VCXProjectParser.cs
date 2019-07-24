using System.Collections.Generic;
using System.IO;
using RepositoryReaders.Path;
using RepositoryReaders.Xml;
using RepositoryScanner.Scanning.FileExplorer;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.StructureParsing.Parsers.Projects
{
    public class VCXProjectParser : IParser<Project>
    {
        private readonly IRepositoryRegistry _repositoryRegistry;
        private readonly IXmlReader _xmlReader;
        private readonly IPathReader _pathReader;

        public VCXProjectParser(IRepositoryRegistry repositoryRegistry, IXmlReader xmlReader, IPathReader pathReader)
        {
            _repositoryRegistry = repositoryRegistry;
            _xmlReader = xmlReader;
            _pathReader = pathReader;
        }

        public Project Parse(string path)
        {
            var directory = _pathReader.GetDirectoryName(path);
            var sourceFiles = new List<SourceFile>();

            _xmlReader.Create(path);

            while (_xmlReader.ReadToFollowing("ClCompile"))
            {
                var baseDirectory = directory;
                var filePath = _xmlReader.GetAttribute("Include");

                if(string.IsNullOrWhiteSpace(filePath))
                {
                    continue;
                }

                filePath = _pathReader.GetFullPath(filePath, baseDirectory);
                sourceFiles.Add(new SourceFile(filePath)
                {
                    Repository = _repositoryRegistry.GetRepositoryFromPath(filePath)
                });
            }

            return new Project(path, sourceFiles);
        }
    }
}