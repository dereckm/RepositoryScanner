using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RepositoryReaders.Text;
using RepositoryScanner.Scanning.Analysis.Analyzers;
using RepositoryScanner.Scanning.Analysis.Analyzers.Files;
using RepositoryScanner.Scanning.Analysis.Analyzers.Structure;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis
{
    public class ConfigurableAnalyzerCollection : IConfigurableAnalyzerCollection
    {
        private readonly IFileReader _fileReader;
        private const string REPOSITORY_CONFIGURATION_FILE = "analyzers.config";
        private readonly HashSet<string> _loadedAnalyzers = new HashSet<string>();

        private readonly List<IAnalyzer<CodeBase>> _structuralAnalyzers = new List<IAnalyzer<CodeBase>>();
        private readonly FileAnalyzer _fileAnalyzer;      

        public ConfigurableAnalyzerCollection(IFileReader fileReader)
        {
            _fileReader = fileReader;
            _fileAnalyzer = new FileAnalyzer(fileReader);
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            if (!_fileReader.Exists(REPOSITORY_CONFIGURATION_FILE))
            {
                throw new FileNotFoundException("The repositories.config file could not be loaded.");
            }

            _structuralAnalyzers.Add(_fileAnalyzer);

            var lines = _fileReader.ReadAllLines(REPOSITORY_CONFIGURATION_FILE);

            foreach (var line in lines)
            {
                if (!line.StartsWith("//"))
                {
                    var analyzer = CreateAnalyzerFromName(line);
                    if (analyzer != null)
                    {
                        _structuralAnalyzers.Add(analyzer);
                    }
                }
            }
        }

        private IAnalyzer<CodeBase> CreateAnalyzerFromName(string line)
        {
            if (!_loadedAnalyzers.Add(line))
            {
                throw new ArgumentException($"The listed analyzer was already added: {line}. Please remove the duplicate from the configuration file.");
            }

            IAnalyzer<CodeBase> analyzer = null;

            switch (line)
            {
                case nameof(EmptyProjectAnalyzer):
                    analyzer =  new EmptyProjectAnalyzer();
                    break;
                case nameof(EmptySolutionAnalyzer):
                    analyzer = new EmptySolutionAnalyzer();
                    break;
                case nameof(FileNeverReferencedAnalyzer):
                    analyzer = new FileNeverReferencedAnalyzer();
                    break;
                case nameof(FileReferencedByMultipleProjectsAnalyzer):
                    analyzer = new FileReferencedByMultipleProjectsAnalyzer();
                    break;
                case nameof(ProjectNotReferencedInAnySolutionAnalyzer):
                    analyzer = new ProjectNotReferencedInAnySolutionAnalyzer();
                    break;
                case nameof(ProjectReferencesFileInAnotherRepositoryAnalyzer):
                    analyzer = new ProjectReferencesFileInAnotherRepositoryAnalyzer();
                    break;
                case nameof(ReferencedFileNotFoundAnalyzer):
                    analyzer = new ReferencedFileNotFoundAnalyzer();
                    break;
                case nameof(MissingObfuscationAttributeAnalyzer):
                    AddAnalyzer(new MissingObfuscationAttributeAnalyzer());
                    break;
                default:
                    throw new ArgumentException($"The listed analyzer does not exist: {line}. Implement it or removed it from the configuration file.");
            }

            return analyzer;
        }

        private void AddAnalyzer(IAnalyzer analyzer)
        {
            _fileAnalyzer.RegisterAnalyzer(analyzer);
        }

        public IEnumerator<IAnalyzer<CodeBase>> GetEnumerator()
        {
            return _structuralAnalyzers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
