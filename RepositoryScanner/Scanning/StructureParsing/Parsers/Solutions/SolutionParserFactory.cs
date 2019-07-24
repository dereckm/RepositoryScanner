using System;
using System.Collections.Generic;
using System.Text;
using RepositoryReaders.Directory;
using RepositoryReaders.Path;
using RepositoryReaders.Text;
using RepositoryReaders.Xml;
using RepositoryScanner.Scanning.FileExplorer;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.StructureParsing.Parsers.Solutions
{
    public class SolutionParserFactory : IParserFactory<Solution>
    {
        private readonly IPathReader _pathReader;
        private readonly IFileReader _fileReader;
        private readonly IDirectoryReader _directoryReader;

        public SolutionParserFactory(IPathReader pathReader, IDirectoryReader directoryReader, IFileReader fileReader)
        {
            _pathReader = pathReader;
            _directoryReader = directoryReader;
            _fileReader = fileReader;
        }

        public IParser<Solution> CreateParser(string path)
        {
            var extension = _pathReader.GetExtension(path);

            switch (extension)
            {
                case ".sln":
                    return new SolutionParser(_fileReader, _directoryReader, _pathReader);
                default:
                    throw new NotImplementedException("Parser not implemented for this type of solution.");
            }
        }
    }
}
