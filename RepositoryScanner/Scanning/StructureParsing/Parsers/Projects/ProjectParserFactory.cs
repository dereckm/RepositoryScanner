using System;
using System.IO;
using RepositoryReaders.Xml;
using RepositoryScanner.Scanning.FileExplorer;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.StructureParsing.Parsers.Projects
{
    public class ProjectParserFactory : IParserFactory<IParser<Project>>
    {
        private readonly IRepositoryRegistry _repositoryRegistry;
        private readonly IXmlReader _xmlReader;

        public ProjectParserFactory(IRepositoryRegistry repositoryRegistry, IXmlReader xmlReader)
        {
            _repositoryRegistry = repositoryRegistry;
            _xmlReader = xmlReader;
        }

        public IParser<Project> CreateParser(string path)
        {
            var extension = Path.GetExtension(path);

            switch (extension)
            {
                case ".csproj":
                    return new CSharpProjectParser(_repositoryRegistry, _xmlReader);
                case ".vcxproj":
                    return new VCXProjectParser(_repositoryRegistry, _xmlReader);
                case ".vcproj":
                    return new VCProjectParser(_repositoryRegistry, _xmlReader);
                default:
                    throw new NotImplementedException("Parser not implemented for this type of project.");
            }
        }
    }
}
