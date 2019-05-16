using System.IO;
using RepositoryReaders.Text;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public class ConfigurableRepositoryRegistry : RepositoryRegistryBase
    {
        private readonly IFileReader _fileReader;
        private const string REPOSITORY_CONFIGURATION_FILE = "repositories.config";

        public ConfigurableRepositoryRegistry(IFileReader fileReader)
        {
            _fileReader = fileReader;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            if (!_fileReader.Exists(REPOSITORY_CONFIGURATION_FILE))
            {
                throw new FileNotFoundException("The repositories.config file could not be loaded.");
            }

            var lines = _fileReader.ReadAllLines(REPOSITORY_CONFIGURATION_FILE);

            foreach (var line in lines)
            {
                if (!line.StartsWith("//"))
                {
                    Registry.Add(new Repository(line));
                }
            }
        }
    }
}
