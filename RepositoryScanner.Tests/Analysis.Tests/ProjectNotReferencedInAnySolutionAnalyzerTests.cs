using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RepositoryScanner.Scanning.Analysis.Analyzers;
using RepositoryScanner.Scanning.Analysis.Analyzers.Structure;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Tests.Analysis.Tests
{
    [TestFixture]
    public class ProjectNotReferencedInAnySolutionAnalyzerTests
    {
        private CodeBase _codeBase;
        private IAnalyzer<CodeBase> _analyzer;

        [SetUp]
        public void SetUp()
        {
            _codeBase = new CodeBase();
            _analyzer = new ProjectNotReferencedInAnySolutionAnalyzer();;
        }

        [Test]
        public void ShouldFindProblemWithNotReferencedProject()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject2.cs", new List<SourceFile>()));
            _codeBase.Solutions.Add(new Solution(@"C:\SomeSolution.sln")
            {
                Projects = new List<Project>()
                {
                    new Project(@"C:\SomeProject.cs", new List<SourceFile>())
                }

            });

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            problems.Should().HaveCount(1);
        }

        [Test]
        public void ShouldNotFindProblemWithAllProjectsReferenced()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject2.cs", new List<SourceFile>()));
            _codeBase.Solutions.Add(new Solution(@"C:\SomeSolution.sln")
            {
                Projects = new List<Project>()
                {
                    new Project(@"C:\SomeProject.cs", new List<SourceFile>()),
                    new Project(@"C:\SomeProject2.cs", new List<SourceFile>())
                }

            });

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().BeEmpty();
        }

        [Test]
        public void ShouldFindAllProblemsWithNotReferencedProjects()
        {
            // Arrange
            _codeBase.Projects.Add(new Project(@"C:\SomeProject.cs", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject2.cs", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject3.cs", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject4.cs", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject5.cs", new List<SourceFile>()));
            _codeBase.Projects.Add(new Project(@"C:\SomeProject6.cs", new List<SourceFile>()));
            _codeBase.Solutions.Add(new Solution(@"C:\SomeSolution.sln")
            {
                Projects = new List<Project>()
                {
                    new Project(@"C:\SomeProject3.cs", new List<SourceFile>()),
                    new Project(@"C:\SomeProject5.cs", new List<SourceFile>())
                }

            });
            _codeBase.Solutions.Add(new Solution(@"C:\SomeSolution2.sln")
            {
                Projects = new List<Project>()
                {
                    new Project(@"C:\SomeProject.cs", new List<SourceFile>()),
                    new Project(@"C:\SomeProject3.cs", new List<SourceFile>())
                }

            });

            // Act
            var problems = _analyzer.FindProblems(_codeBase);

            // Assert
            problems.Should().HaveCount(3);
        }
    }
}
