using System;
using System.IO;
using RepositoryReaders.Directory;
using RepositoryReaders.Text;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public class ConfigurableRepositoryRegistry : RepositoryRegistryBase
    {
        private readonly IFileReader _fileReader;
        private readonly IDirectoryReader _directoryReader;
        private const string REPOSITORY_CONFIGURATION_FILE = "repositories.config";

        public ConfigurableRepositoryRegistry(IFileReader fileReader, IDirectoryReader directoryReader)
        {
            _fileReader = fileReader;
            _directoryReader = directoryReader;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            if (!_fileReader.FileExists(REPOSITORY_CONFIGURATION_FILE))
            {
                throw CreateConfigurationNotFoundException(REPOSITORY_CONFIGURATION_FILE);
            }

            var lines = _fileReader.ReadAllLines(REPOSITORY_CONFIGURATION_FILE);

            foreach (var line in lines)
            {
                if (!line.StartsWith("//"))
                {
                    var directory = line.Trim();

                    if (_directoryReader.DirectoryExists(directory))
                    {
                        Registry.Add(new Repository(directory));
                    }
                    else
                    {
                        throw CreateDirectoryNotFoundException(directory);
                    }
                }
            }
        }

        private static InvalidRepositoryConfigurationException CreateConfigurationNotFoundException(string configurationFile)
        {
            var innerException = new FileNotFoundException($"The {configurationFile} file could not be loaded.");

            return CreateInvalidRepositoryConfigurationException(innerException);
        }

        private static InvalidRepositoryConfigurationException CreateDirectoryNotFoundException(string directory)
        {
            var innerException =
                new DirectoryNotFoundException(
                    $"The specified repository in the configuration file was not found: {directory}");

            return CreateInvalidRepositoryConfigurationException(innerException);
        }

        private static InvalidRepositoryConfigurationException CreateInvalidRepositoryConfigurationException(Exception innerException)
        {
            return new InvalidRepositoryConfigurationException("Invalid repository configuration", innerException);
        }
    }
}
