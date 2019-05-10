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
    public class EmptyProjectAnalyzerTests
    {
        private CodeBase _codeBase;
        private IAnalyzer _analyzer;

        [SetUp]
        public void SetUp()
        {
            _codeBase = new CodeBase();
            _analyzer = new EmptyProjectAnalyzer();;
        }

        [Test]
        public void ShouldFindProblemWhenEmpty()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject.csproj", new List<SourceFile>()));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(1);
        }

        [Test]
        public void ShouldNotFindProblemWhenNotEmpty()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFakePath\file.cs") }));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(0);
        }

        [Test]
        public void ShouldFindOneProblemWithOneEmptyProject()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFakePath\file.cs") }));
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject2.csproj", new List<SourceFile>()));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);
            
            // Assert
            problems.Should().HaveCount(1);
        }

        [Test]
        public void ShouldFindAllProblemsWithMultipleEmptyProjects()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFakePath\file.cs") }));
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject2.csproj", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject3.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFakePath\file.cs") }));
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject4.csproj", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject5.csproj", new List<SourceFile>() { new SourceFile(@"C:\SomeFakePath\file.cs") }));
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject6.csproj", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeFakePath\SomeFakeProject7.csproj", new List<SourceFile>()));

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(4);
        }
    }
}
