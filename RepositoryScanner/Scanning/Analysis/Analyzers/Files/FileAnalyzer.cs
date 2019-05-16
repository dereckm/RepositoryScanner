using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RepositoryReaders.Text;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.Analysis.Analyzers.Files
{
    public class FileAnalyzer : CSharpSyntaxWalker, IAnalyzer<CodeBase>
    {
        private readonly IFileReader _fileReader;
        private readonly ConcurrentQueue<Problem> _problems = new ConcurrentQueue<Problem>();
        private readonly List<IAnalyzer> _analyzers = new List<IAnalyzer>();

        public FileAnalyzer(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public IEnumerable<Problem> FindProblems(CodeBase codeBase)
        {
            if (_analyzers.Count == 0)
            {
                yield break;
            }

            _problems.Clear();

            Parallel.ForEach(codeBase.SourceFiles.Select(x => x.Path).Where(p => Path.GetExtension(p) == ".cs"), (path) =>
            {
                var content = _fileReader.ReadAllText(path);

                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(content);
                CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

                Visit(root);
            });

            foreach (var problem in _problems)
            {
                yield return problem;
            }
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            foreach (var classAnalyzer in _analyzers.OfType<IAnalyzer<ClassDeclarationSyntax>>())
            {
                var problems = classAnalyzer.FindProblems(node);
                foreach (var problem in problems)
                {
                    _problems.Enqueue(problem);
                }
            }

            base.VisitClassDeclaration(node);
        }

        public void RegisterAnalyzer(IAnalyzer analyzer)
        {
            _analyzers.Add(analyzer);
        }
    }
}
