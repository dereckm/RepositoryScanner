using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RepositoryScanner.Scanning.Analysis.Analyzers;
using RepositoryScanner.Scanning.Analysis.Analyzers.Structure;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Tests.Analysis.Tests
{
    [TestFixture]
    public class ReferencedFileNotFoundAnalyzerTests
    {
        private CodeBase _codeBase;
        private IAnalyzer<CodeBase> _analyzer;

        [SetUp]
        public void SetUp()
        {
            _codeBase = new CodeBase();
            _analyzer = new ReferencedFileNotFoundAnalyzer();
        }

        [Test]
        public void ShouldFindProblemForOneReferencedFileNotFound()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFile.cs"), new SourceFile(@"C:\SomeFile2.cs")}));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeFile.cs"));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(1);
        }

        [Test]
        public void ShouldNotFindProblemWhenAllFilesReferencedExist()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFile.cs"), new SourceFile(@"C:\SomeFile2.cs")}));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeFile.cs"));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeFile2.cs"));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().BeEmpty();
        }

        [Test]
        public void ShouldFindAllReferencedFileNotFoundProblems()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFile.cs"), new SourceFile(@"C:\SomeFile2.cs")}));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject2.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFile2.cs"), new SourceFile(@"C:\SomeFile3.cs")}));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject3.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFile.cs"), new SourceFile(@"C:\SomeFile6.cs")}));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject4.csproj", new List<SourceFile>() {new SourceFile(@"C:\SomeFile2.cs"), new SourceFile(@"C:\SomeFile.cs")}));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeFile.cs"));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeFile2.cs"));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(2);
        }
    }
}
