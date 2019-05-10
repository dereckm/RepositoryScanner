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
    public class FileNeverReferencedAnalyzerTests
    {
        private CodeBase _codeBase;
        private IAnalyzer _analyzer;

        [SetUp]
        public void SetUp()
        {
            _codeBase = new CodeBase();
            _analyzer = new FileNeverReferencedAnalyzer();
        }

        [Test]
        public void ShouldNotFindProblemWhenAllFilesReferenced()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile.cs")}));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeSourceFile.cs"));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().BeEmpty();
        }

        [Test]
        public void ShouldFindProblemWhenOneFileNotReferenced()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile.cs")}));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeSourceFile.cs"));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeSourceFile2.cs"));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(1);
        }

        [Test]
        public void ShouldFindAllNotReferencedProblems()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile5.cs")}));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject2.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile2.cs"), new SourceFile(@"C:\SomeSourceFile4.cs")}));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeSourceFile.cs"));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeSourceFile2.cs"));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeSourceFile3.cs"));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeSourceFile4.cs"));
            _codeBase.SourceFiles.Add(new SourceFile(@"C:\SomeSourceFile5.cs"));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            // SomeSourceFile, SomeSourceFile3 are not referenced
            problems.Should().HaveCount(2);
        }
    }
}
