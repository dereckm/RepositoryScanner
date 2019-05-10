using System.IO;
using RepositoryScanner.V2.Structure;
using RepositoryScanner.V2.StructureParsing;

namespace RepositoryScanner.V2.FileExplorer
{
    public class FileVisitor
    {
        private readonly  CodeBase _codeBase = new CodeBase();
        private readonly IFileVisitorFilter _defaultFileVisitorFilter;
        private readonly IParser<Solution> _solutionParser;

        public FileVisitor(
            IFileVisitorFilter defaultFileVisitorFilter, 
            IParser<Solution> solutionParser)
        {
            _defaultFileVisitorFilter = defaultFileVisitorFilter;
            _solutionParser = solutionParser;
        }

        public void Visit(string directoryPath)
        {
            _codeBase.Repositories.Add(new Repository(directoryPath));
            VisitFiles(directoryPath);
        }

        private void VisitFiles(string directoryPath)
        {
            foreach (var file in Directory.EnumerateFiles(directoryPath))
            {
                VisitFile(file);

                var extension = Path.GetExtension(file);

                if (_defaultFileVisitorFilter.IsSolution(extension))
                {
                    VisitSolution(file);
                }
                else if (_defaultFileVisitorFilter.IsProject(extension))
                {
                    VisitProject(file);
                }
                else if (_defaultFileVisitorFilter.IsSourceFile(extension))
                {
                    VisitSourceFile(file);
                }
            }

            foreach (var directory in Directory.EnumerateDirectories(directoryPath))
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
            var parser = new CSharpProjectParser2(_codeBase.CurrentRepository);
            var project = parser.Parse(projectPath);
            project.Repository = _codeBase.CurrentRepository;

            _codeBase.Projects.Add(project);
        }

        protected virtual void VisitSourceFile(string sourceFilePath)
        {
            if (!IsTemporaryFile(sourceFilePath))
            {
                _codeBase.SourceFiles.Add(new SourceFile(sourceFilePath) { Repository =  _codeBase.CurrentRepository });
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
