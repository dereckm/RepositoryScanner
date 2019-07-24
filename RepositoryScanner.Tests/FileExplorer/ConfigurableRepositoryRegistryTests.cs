using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RepositoryReaders.Directory;
using RepositoryReaders.Text;
using RepositoryScanner.Scanning.FileExplorer;

namespace RepositoryScanner.Tests.FileExplorer
{
    [TestFixture]
    public class ConfigurableRepositoryRegistryTests
    {
        private IRepositoryRegistry _repositoryRegistry;
        private Mock<IFileReader> _fileReaderMock;
        private Mock<IDirectoryReader> _directoryReaderMock;

        [SetUp]
        public void SetUp()
        {
            _fileReaderMock = new Mock<IFileReader>();
            const string validPath = @"repositories.config";
            _fileReaderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns<string>((s) => s == validPath);
            _fileReaderMock.Setup(x => x.ReadAllLines(It.IsAny<string>())).Returns(new[]
            {
                "// This is some header comment",
                @"C:\This\Is\SomeFirstRepository",
                @"// C:\This\Is\SomeDisabledRepository",
                @"C:\This\Is\SomeOtherRepository"
            });

            _directoryReaderMock = new Mock<IDirectoryReader>();
            _directoryReaderMock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(true);
        }

        [Test]
        public void ShouldThrowIfConfigurationFileDoesNotExist()
        {
            // Arrange
            const string validPath = @"SomeInvalidFilePath.config";
            _fileReaderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns<string>((s) => s == validPath);

            // Act 
            Action act = () => _repositoryRegistry = new ConfigurableRepositoryRegistry(_fileReaderMock.Object, _directoryReaderMock.Object);

            // Assert
            act.Should().Throw<InvalidRepositoryConfigurationException>().WithInnerException<FileNotFoundException>()
                .WithMessage("The repositories.config file could not be loaded.");
        }

        [Test]
        public void ShouldThrowIfRepositoryDoesNotExist()
        {
            // Arrange
            _directoryReaderMock.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(false);
            const string directory = @"C:\This\Is\SomeFirstRepository";

            // Act
            Action act = () =>
                _repositoryRegistry =
                    new ConfigurableRepositoryRegistry(_fileReaderMock.Object, _directoryReaderMock.Object);

            // Assert
            act.Should().Throw<InvalidRepositoryConfigurationException>()
                .WithInnerException<DirectoryNotFoundException>().WithMessage($"The specified repository in the configuration file was not found: {directory}");
        }

        [Test]
        public void ShouldGetPathFromRepositoriesProperly()
        {
            // Act 
            _repositoryRegistry = new ConfigurableRepositoryRegistry(_fileReaderMock.Object, _directoryReaderMock.Object);

            // Assert
            var repositoryFromPath = _repositoryRegistry.GetRepositoryFromPath(@"C:\This\Is\SomeFirstRepository\SomeFolder\SomeFile.cs");
            repositoryFromPath.Path.Should().Be(@"C:\This\Is\SomeFirstRepository");
        }

        [Test]
        public void ShouldReturnNullRepositoryIfOutsideRepository()
        {
            // Act 
            _repositoryRegistry = new ConfigurableRepositoryRegistry(_fileReaderMock.Object, _directoryReaderMock.Object);

            // Assert
            var repositoryFromPath = _repositoryRegistry.GetRepositoryFromPath(@"C:\This\Is\FolderOutsideRepository\SomeFolder\SomeFile.cs");
            repositoryFromPath.Should().BeNull(because: "Scanned files should be inside the repository.");
        }

        [Test]
        public void ShouldReadAllRepositoriesProperly()
        {
            // Act
            _repositoryRegistry = new ConfigurableRepositoryRegistry(_fileReaderMock.Object, _directoryReaderMock.Object);

            // Assert
            _repositoryRegistry.Count().Should().Be(2);

            var firstRepository = _repositoryRegistry.First();
            firstRepository.Path.Should().Be(@"C:\This\Is\SomeFirstRepository");
            firstRepository.Name.Should().Be("SomeFirstRepository");
            var current = _repositoryRegistry.Current;
            current.Path.Should().Be(firstRepository.Path);

            var secondRepository = _repositoryRegistry.Skip(1).Take(1).First();
            secondRepository.Path.Should().Be(@"C:\This\Is\SomeOtherRepository");
            secondRepository.Name.Should().Be("SomeOtherRepository");
            current = _repositoryRegistry.Current;
            current.Path.Should().Be(secondRepository.Path);
        }
    }
}
