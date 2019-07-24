using System.IO;
using RepositoryReaders.Directory;
using RepositoryReaders.Path;
using RepositoryScanner.Scanning.Structure;
using RepositoryScanner.Scanning.StructureParsing.Parsers;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public class RepositoryVisitor : IRepositoryVisitor
    {
        private readonly  CodeBase _codeBase = new CodeBase();
        private readonly IRepositoryVisitorFilter _defaultRepositoryVisitorFilter;
        private readonly IParserFactory<Solution> _solutionParserFactory;
        private readonly IParserFactory<Project> _projectParserFactory;
        private readonly IDirectoryReader _directoryReader;
        private readonly IPathReader _pathReader;
        private readonly IRepositoryRegistry _repositoryRegistry;

        public RepositoryVisitor(
            IRepositoryVisitorFilter defaultRepositoryVisitorFilter, 
            IParserFactory<Solution> solutionParserFactory, 
            IRepositoryRegistry repositoryRegistry, 
            IParserFactory<Project> projectParserFactory,
            IDirectoryReader directoryReader,
            IPathReader pathReader)
        {
            _defaultRepositoryVisitorFilter = defaultRepositoryVisitorFilter;
            _solutionParserFactory = solutionParserFactory;
            _repositoryRegistry = repositoryRegistry;
            _projectParserFactory = projectParserFactory;
            _directoryReader = directoryReader;
            _pathReader = pathReader;
        }

        public void Visit()
        {
            foreach(var repository in _repositoryRegistry)
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

                var extension = _pathReader.GetExtension(file);

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
            var parser = _solutionParserFactory.CreateParser(solutionPath);

            var solution = parser.Parse(solutionPath);

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

            var fileName = _pathReader.GetFileName(sourceFilePath);

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
