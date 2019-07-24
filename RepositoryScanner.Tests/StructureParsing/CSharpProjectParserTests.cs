using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RepositoryReaders.Path;
using RepositoryReaders.Xml;
using RepositoryScanner.Scanning.FileExplorer;
using RepositoryScanner.Scanning.Structure;
using RepositoryScanner.Scanning.StructureParsing;
using RepositoryScanner.Scanning.StructureParsing.Parsers.Projects;

namespace RepositoryScanner.Tests.StructureParsing
{
    [TestFixture]
    public class CSharpProjectParserTests
    {
        private CSharpProjectParser _cSharpProjectParser;
        private List<string> _testInputs;
        private int _inputIndex;

        [SetUp]
        public void SetUp()
        {
            _testInputs = new List<string>()
            {
                @"C:\Tests\SomeFileTests.cs",
                @"C:\Source\SomeFile.cs",
                @"C:\Tests\SomeOtherFileTests.cs",
                @"C:\Source\SomeOtherFile.cs",
                @"C:\Source\SomeLastFile.cs"
            };
            _inputIndex = -1;


            var registry = new Mock<IRepositoryRegistry>();
            registry.Setup(x => x.GetRepositoryFromPath(It.IsAny<string>())).Returns<string>((path) => new Repository(Path.GetDirectoryName(path)));

            var xmlReader = new Mock<IXmlReader>();
            xmlReader.Setup(x => x.ReadToFollowing("Compile")).Returns(() =>
            {
                _inputIndex++;
                return _inputIndex < _testInputs.Count;
            });
            xmlReader.Setup(x => x.GetAttribute("Include")).Returns(() => _testInputs[_inputIndex]);
            var pathReader = new DefaultPathReader();


            _cSharpProjectParser = new CSharpProjectParser(registry.Object, xmlReader.Object, pathReader);
        }

        [Test]
        public void ShouldReturnZeroSourceFilesForNoCompile()
        {
            // Arrange
            _testInputs.Clear();
            const string path = @"C:\Source\Project.csproj";

            // Act
            var project = _cSharpProjectParser.Parse(path);

            // Assert
            project.SourceFiles.Should().NotBeNull();
            project.SourceFiles.Any().Should().BeFalse();
            project.Path.Should().Be(path);
        }

        [Test]
        public void ShouldFindAllCompileIncludes()
        {
            // Arrange
            const string path = @"C:\Source\Project.csproj";

            // Act
            var project = _cSharpProjectParser.Parse(path);

            // Assert
            project.SourceFiles.Should().NotBeNull();
            project.SourceFiles.Count().Should().Be(5);
            project.SourceFiles.Skip(2).Take(1).First().Path.Should().Be(@"C:\Tests\SomeOtherFileTests.cs");
            project.SourceFiles.Skip(3).Take(1).First().Repository.Path.Should()
                .Be(Path.GetDirectoryName(@"C:\Source\SomeOtherFile.cs"));
            project.Path.Should().Be(path);
        }
    }
}
