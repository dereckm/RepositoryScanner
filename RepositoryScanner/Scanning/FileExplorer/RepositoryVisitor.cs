using System.IO;
using RepositoryReaders.Directory;
using RepositoryScanner.Scanning.Structure;
using RepositoryScanner.Scanning.StructureParsing.Parsers;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public class RepositoryVisitor
    {
        private readonly  CodeBase _codeBase = new CodeBase();
        private readonly IRepositoryVisitorFilter _defaultRepositoryVisitorFilter;
        private readonly IParser<Solution> _solutionParser;
        private readonly IParserFactory<IParser<Project>> _projectParserFactory;
        private readonly IDirectoryReader _directoryReader;
        private readonly IRepositoryRegistry _repositoryRegistry;

        public RepositoryVisitor(
            IRepositoryVisitorFilter defaultRepositoryVisitorFilter, 
            IParser<Solution> solutionParser, 
            IRepositoryRegistry repositoryRegistry, 
            IParserFactory<IParser<Project>> projectParserFactory,
            IDirectoryReader directoryReader)
        {
            _defaultRepositoryVisitorFilter = defaultRepositoryVisitorFilter;
            _solutionParser = solutionParser;
            _repositoryRegistry = repositoryRegistry;
            _projectParserFactory = projectParserFactory;
            _directoryReader = directoryReader;
        }

        public void Visit()
        {
            while (_repositoryRegistry.TryGetNext(out Repository repository))
            {
                _codeBase.Repositories.Add(repository);
                VisitFiles(repository.Path);
            }
        }

        private void VisitFiles(string directoryPath)
        {
            foreach (var file in _directoryReader.EnumerateFiles(directoryPath))
            {
                VisitFile(file);

                var extension = Path.GetExtension(file);

                if (_defaultRepositoryVisitorFilter.IsSolutionFile(extension))
                {
                    VisitSolution(file);
                }
                else if (_defaultRepositoryVisitorFilter.IsProjectFile(extension))
                {
                    VisitProject(file);
                }
                else if (_defaultRepositoryVisitorFilter.IsSourceFile(extension))
                {
                    VisitSourceFile(file);
                }
            }

            foreach (var directory in _directoryReader.EnumerateDirectories(directoryPath))
            {
                VisitFiles(directory);
            }
        }

        protected virtual void VisitSolution(string solutionPath)
        {
            var solution = _solutionParser.Parse(solutionPath);

            _codeBase.Solutions.Add(solution);
        }

        protected virtual void VisitProject(string projectPath)
        {
            var parser = _projectParserFactory.CreateParser(projectPath);

            var project = parser.Parse(projectPath);
            project.Repository = _repositoryRegistry.Current;

            _codeBase.Projects.Add(project);
        }

        protected virtual void VisitSourceFile(string sourceFilePath)
        {
            if (!IsTemporaryFile(sourceFilePath))
            {
                _codeBase.SourceFiles.Add(new SourceFile(sourceFilePath) { Repository =  _repositoryRegistry.Current });
            }
        }

        private bool IsTemporaryFile(string sourceFilePath)
        {
            const string temporaryFilePrefix = "TemporaryGeneratedFile";

            var fileName = Path.GetFileName(sourceFilePath);

            var fileNameParts = fileName.Split('_');

            return fileNameParts[0] == temporaryFilePrefix;
        }

        protected virtual void VisitFile(string filePath)
        {
        }

        public CodeBase GetCodeBase()
        {
            return _codeBase;
        }
    }
}
