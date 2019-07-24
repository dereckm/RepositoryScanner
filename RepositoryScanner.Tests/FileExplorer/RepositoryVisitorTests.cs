using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RepositoryReaders.Directory;
using RepositoryReaders.Path;
using RepositoryScanner.Scanning.FileExplorer;
using RepositoryScanner.Scanning.Structure;
using RepositoryScanner.Scanning.StructureParsing.Parsers;

namespace RepositoryScanner.Tests.FileExplorer
{
    [TestFixture]
    public class RepositoryVisitorTests
    {
        private IRepositoryVisitor _repositoryVisitor;
        private Mock<IRepositoryVisitorFilter> _repositoryVisitorFilterMock;
        private Mock<IRepositoryRegistry> _repositoryRegistryMock;
        private Mock<IParserFactory<Project>> _projectParserFactoryMock;
        private Mock<IParserFactory<Solution>> _solutionParserFactoryMock;
        private IPathReader _pathReader;
        private Mock<IDirectoryReader> _directoryReaderMock;
        private Dictionary<string, List<string>> _dictionaryOfFilesPerFolder = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> _dictionaryOfDirectoriesInDirectory = new Dictionary<string, List<string>>();
        private IEnumerator<Repository> _currentEnumerator;

        [SetUp]
        public void SetUp()
        {
            _pathReader = new DefaultPathReader();

            #region Setup _directoryReaderMock
                        var firstSetOfFiles = new List<string>
            {
                @"C:\Some\Repository\Project\File1.csproj",
                @"C:\Some\Repository\Project\File2.cs",
                @"C:\Some\Repository\Project\File3.cs",
                @"C:\Some\Repository\Project\File4.cs",
                @"C:\Some\Repository\Project\File5.cs",
                @"C:\Some\Repository\Project\Solution.sln",
            };

            var secondSetOfFiles = new List<string>
            {
                @"C:\Some\Repository\Project2\File21.csproj",
                @"C:\Some\Repository\Project2\File22.cs",
                @"C:\Some\Repository\Project2\File23.cs",
                @"C:\Some\Repository\Project2\File24.cs",
                @"C:\Some\Repository\Project2\File25.cs",
            };

            var thirdSetOfFiles = new List<string>
            {
                @"C:\Some\OtherRepo\Project\File31.csproj",
                @"C:\Some\OtherRepo\Project\File32.cs",
                @"C:\Some\OtherRepo\Project\File33.cs",
                @"C:\Some\OtherRepo\Project\File34.cs",
                @"C:\Some\OtherRepo\Project\File35.cs",
                @"C:\Some\OtherRepo\Project\OtherSolution.sln",
            };

            var fourthSetOfFiles = new List<string>
            {
                @"C:\Some\OtherRepo\Project\Project\File41.csproj",
                @"C:\Some\OtherRepo\Project\Project\File42.cs",
                @"C:\Some\OtherRepo\Project\Project\File43.cs",
                @"C:\Some\OtherRepo\Project\Project\File44.cs",
                @"C:\Some\OtherRepo\Project\Project\File45.cs",
            };

            _dictionaryOfFilesPerFolder = new Dictionary<string, List<string>>();
            _dictionaryOfFilesPerFolder.Add(@"C:\Some\Repository\Project", firstSetOfFiles);
            _dictionaryOfFilesPerFolder.Add(@"C:\Some\Repository\Project2", secondSetOfFiles);
            _dictionaryOfFilesPerFolder.Add(@"C:\Some\OtherRepo\Project", thirdSetOfFiles);
            _dictionaryOfFilesPerFolder.Add(@"C:\Some\OtherRepo\Project\Project", fourthSetOfFiles);

            _dictionaryOfDirectoriesInDirectory = new Dictionary<string, List<string>>();
            _dictionaryOfDirectoriesInDirectory.Add(@"C:\Some\Repository", new List<string>()
            {
                @"C:\Some\Repository\Project",
                @"C:\Some\Repository\Project2"
            });
            _dictionaryOfDirectoriesInDirectory.Add(@"C:\Some\OtherRepo", new List<string>()
            {
                @"C:\Some\OtherRepo\Project"
            });
            _dictionaryOfDirectoriesInDirectory.Add(@"C:\Some\OtherRepo\Project", new List<string>()
            {
                @"C:\Some\OtherRepo\Project\Project"
            });

            _directoryReaderMock = new Mock<IDirectoryReader>();
            _directoryReaderMock.Setup(x => x.EnumerateFiles(It.IsAny<string>())).Returns<string>((s =>
                _dictionaryOfFilesPerFolder.TryGetValue(s, out var files) ? files : new List<string>()));
            _directoryReaderMock.Setup(x => x.EnumerateDirectories(It.IsAny<string>())).Returns<string>(s => _dictionaryOfDirectoriesInDirectory.TryGetValue(s, out var directories)
                ? directories
                : new List<string>());
           
            #endregion

            #region Setup _IParserMocks
            _projectParserFactoryMock = new Mock<IParserFactory<Project>>();
            var projectParserMock = new Mock<IParser<Project>>();
            projectParserMock.Setup(x => x.Parse(It.IsAny<string>()))
                .Returns<string>(s => new Project(s, new List<SourceFile>()));
            _projectParserFactoryMock.Setup(x => x.CreateParser(It.IsAny<string>())).Returns(projectParserMock.Object);

            _solutionParserFactoryMock = new Mock<IParserFactory<Solution>>();
            var solutionParserMock = new Mock<IParser<Solution>>();
            solutionParserMock.Setup(x => x.Parse(It.IsAny<string>()))
                .Returns<string>(s => new Solution(s));
            _solutionParserFactoryMock.Setup(x => x.CreateParser(It.IsAny<string>())).Returns(solutionParserMock.Object);
            #endregion

            _repositoryVisitorFilterMock = new Mock<IRepositoryVisitorFilter>();
            // TODO: Check the strings for real
            _repositoryVisitorFilterMock.Setup(x => x.IsSourceFile(It.IsAny<string>())).Returns<string>(s => Path.GetExtension(s) == ".cs");
            _repositoryVisitorFilterMock.Setup(x => x.IsProjectFile(It.IsAny<string>())).Returns<string>(s => Path.GetExtension(s) == ".csproj");
            _repositoryVisitorFilterMock.Setup(x => x.IsSolutionFile(It.IsAny<string>())).Returns<string>(s => Path.GetExtension(s) == ".sln");

            _repositoryRegistryMock = new Mock<IRepositoryRegistry>();

            var repositories = new List<Repository>
            {
                new Repository(@"C:\Some\Repository"),
                new Repository(@"C:\Some\OtherRepo")
            };

            _repositoryRegistryMock.Setup(x => x.GetEnumerator()).Returns(() =>
                {
                    _currentEnumerator = repositories.GetEnumerator();
                    return _currentEnumerator;
                });

            _repositoryRegistryMock.SetupGet(x => x.Current).Returns(() => _currentEnumerator.Current);

            _repositoryVisitor = new RepositoryVisitor(_repositoryVisitorFilterMock.Object, _solutionParserFactoryMock.Object, _repositoryRegistryMock.Object, _projectParserFactoryMock.Object,
                _directoryReaderMock.Object, _pathReader);
        }

        [Test]
        public void ShouldVisitAllExpectedFiles()
        {

            // Act
            _repositoryVisitor.Visit();

            // Assert
            _repositoryVisitor.GetCodeBase().Repositories.Should().HaveCount(2);
            _repositoryVisitor.GetCodeBase().Solutions.Should().HaveCount(2);
            _repositoryVisitor.GetCodeBase().Projects.Should().HaveCount(4);
            _repositoryVisitor.GetCodeBase().SourceFiles.Should().HaveCount(16);
        }
    }
}
