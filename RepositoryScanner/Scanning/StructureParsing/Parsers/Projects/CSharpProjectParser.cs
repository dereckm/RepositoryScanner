﻿using System.Collections.Generic;
using System.IO;
using RepositoryReaders.Xml;
using RepositoryScanner.Scanning.FileExplorer;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.StructureParsing.Parsers.Projects
{
    public class CSharpProjectParser : IParser<Project>
    {
        private readonly IRepositoryRegistry _repositoryRegistry;
        private readonly IXmlReader _xmlReader;

        public CSharpProjectParser(IRepositoryRegistry repositoryRegistry, IXmlReader xmlReader)
        {
            _repositoryRegistry = repositoryRegistry;
            _xmlReader = xmlReader;
        }

        public Project Parse(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var sourceFiles = new List<SourceFile>();

            _xmlReader.Create(path);

            while (_xmlReader.ReadToFollowing("Compile"))
            {
                var baseDirectory = directory;
                var filePath = _xmlReader.GetAttribute("Include");
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