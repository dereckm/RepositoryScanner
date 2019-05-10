using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using RepositoryScanner.V2.Analysis;
using RepositoryScanner.V2.Structure;

namespace RepositoryScanner.Tests.Analysis.Tests
{
    [TestFixture]
    public class ProjectReferencesFileInAnotherRepositoryAnalyzerTests
    {
        private CodeBase _codeBase;
        private IAnalyzer _analyzer;
        private Repository _repository;

        [SetUp]
        public void SetUp()
        {
            _codeBase = new CodeBase();
            _analyzer = new ProjectReferencesFileInAnotherRepositoryAnalyzer();
            _repository = new Repository(@"C:\Design");
        }

        [Test]
        public void ShouldNotFindProblemsWhenSameRepository()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new SourceFile[] { new SourceFile("SomeFile.cs") { Repository =  _repository} }) { Repository = _repository });

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().BeEmpty();
        }

        [Test]
        public void ShouldFindProblemWhenReferencingFileFromOtherRepository()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new SourceFile[] { new SourceFile("SomeFile.cs") { Repository =  _repository} }) { Repository = new Repository(@"C:\Common") });

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(1);
        }

        [Test]
        public void ShouldFindAllReferencingOtherRepositoryProblems()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new SourceFile[] { new SourceFile("SomeFile.cs") { Repository =  _repository} }) { Repository = new Repository(@"C:\Common") });
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new SourceFile[] { new SourceFile("SomeFile2.cs") { Repository =  _repository} }) { Repository = new Repository(@"C:\Design") });
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new SourceFile[] { new SourceFile("SomeFile3.cs") { Repository =  _repository} }) { Repository = new Repository(@"C:\Common") });
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new SourceFile[] { new SourceFile("SomeFile4.cs") { Repository =  _repository} }) { Repository = new Repository(@"C:\Design") });
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new SourceFile[] { new SourceFile("SomeFile5.cs") { Repository =  new Repository(@"C:\Common")} }) { Repository = new Repository(@"C:\Common") });

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(2);
        }
    }
}
