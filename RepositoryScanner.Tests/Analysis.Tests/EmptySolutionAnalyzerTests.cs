using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RepositoryScanner.Scanning.Analysis.Analyzers;
using RepositoryScanner.Scanning.Analysis.Analyzers.Structure;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Tests.Analysis.Tests
{
    [TestFixture]
    public class EmptySolutionAnalyzerTests
    {
        private CodeBase _codeBase;
        private IAnalyzer<CodeBase> _analyzer;

        [SetUp]
        public void SetUp()
        {
            _codeBase = new CodeBase();
            _analyzer = new EmptySolutionAnalyzer();;
        }

        [Test]
        public void ShouldFindProblemWhenSolutionContainsNoProject()
        {
            // Arrange
            _codeBase.Solutions.Add(new Solution(@"C:\MySolution.sln"));
            
            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(1);
        }

        [Test]
        public void ShouldNotFindProblemWhenSolutionHasProject()
        {
            // Arrange
            _codeBase.Solutions.Add(new Solution(@"C:\MySolution.sln")
            {
                Projects = new List<Project>()
                {
                    new Project(@"C:\SomeProject.csproj", new List<SourceFile>())
                }
            });
            
            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(0);
        }


        [Test]
        public void ShouldFindAllExpectedProblems()
        {
            // Arrange
            _codeBase.Solutions.Add(new Solution(@"C:\MySolution.sln")
            {
                Projects = new List<Project>()
                {
                    new Project(@"C:\SomeProject.csproj", new List<SourceFile>())
                }
            });
            _codeBase.Solutions.Add(new Solution(@"C:\MySolution2.sln")
            {
                Projects = new List<Project>()
                {
                    new Project(@"C:\SomeProject.csproj", new List<SourceFile>())
                }
            });
            _codeBase.Solutions.Add(new Solution(@"C:\MySolution3.sln"));
            _codeBase.Solutions.Add(new Solution(@"C:\MySolution4.sln"));
            
            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(2);
        }
    }
}
