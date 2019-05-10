using RepositoryScanner.V2.Analysis;
using RepositoryScanner.V2.Structure;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace RepositoryScanner.Tests.Analysis.Tests
{
    [TestFixture]
    public class FileReferencedByMultipleProjectsAnalyzerTests
    {
        private CodeBase _codeBase;
        private IAnalyzer _analyzer;

        [SetUp]
        public void SetUp()
        {
            _codeBase = new CodeBase();
            _analyzer = new FileReferencedByMultipleProjectsAnalyzer();
        }

        [Test]
        public void ShouldFindProblemWhenFileReferenceMultipleTimes()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile.cs")}));
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject2.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile.cs")}));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(1);
        }

        [Test]
        public void ShouldNotFindProblemWhenNoFileReferenceMultipleTimes()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile.cs")}));
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject2.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile2.cs")}));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(0);
        }

        [Test]
        public void ShouldFindAllExpectedProblems()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile.cs")}));
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject2.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile2.cs")}));
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject3.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile.cs")}));
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject4.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile.cs")}));
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject5.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile.cs"), new SourceFile(@"C:\SomeSourceFile2.cs")}));
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject6.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile3.cs")}));
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject7.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile4.cs")}));
            _codeBase.Projects.Add(new Project(@"C\SomePath\SomeProject8.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeSourceFile5.cs"), new SourceFile(@"C:\SomeSourceFile4.cs")}));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            // Looking for overlaps, so SomeSourceFile, SomeSourceFile2 and SomeSourceFile4 are culprits.
            problems.Should().HaveCount(3);
        }
    }
}
