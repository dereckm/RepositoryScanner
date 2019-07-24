using System;
using System.IO;
using RepositoryReaders.Path;
using RepositoryReaders.Xml;
using RepositoryScanner.Scanning.FileExplorer;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.StructureParsing.Parsers.Projects
{
    public class ProjectParserFactory : IParserFactory<Project>
    {
        private readonly IRepositoryRegistry _repositoryRegistry;
        private readonly IXmlReader _xmlReader;
        private readonly IPathReader _pathReader;

        public ProjectParserFactory(IRepositoryRegistry repositoryRegistry, IXmlReader xmlReader, IPathReader pathReader)
        {
            _repositoryRegistry = repositoryRegistry;
            _xmlReader = xmlReader;
            _pathReader = pathReader;
        }

        public IParser<Project> CreateParser(string path)
        {
            var extension = _pathReader.GetExtension(path);

            switch (extension)
            {
                case ".csproj":
                    return new CSharpProjectParser(_repositoryRegistry, _xmlReader, _pathReader);
                case ".vcxproj":
                    return new VCXProjectParser(_repositoryRegistry, _xmlReader, _pathReader);
                case ".vcproj":
                    return new VCProjectParser(_repositoryRegistry, _xmlReader, _pathReader);   
                default:
                    throw new NotImplementedException("Parser not implemented for this type of project.");
            }
        }
    }
}
