using System;
using System.Collections.Generic;
using System.IO;
using RepositoryReaders.Text;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public class DefaultRepositoryVisitorFilter : IRepositoryVisitorFilter
    {
        private readonly IFileReader _fileReader;

        private class ConfigurationType
        {
            public readonly string Separator;
            public readonly Func<string, bool> ValidationFunction;
            public readonly HashSet<string> Extensions;

            public ConfigurationType(string separator, Func<string, bool> validationFunction, HashSet<string> extensions)
            {
                Separator = separator;
                ValidationFunction = validationFunction;
                Extensions = extensions;
            }
        }

        private const string REPOSITORY_CONFIGURATION_FILE = "files.config";

        private readonly List<ConfigurationType> _configurationTypes;

        public DefaultRepositoryVisitorFilter(IFileReader fileReader)
        {
            _fileReader = fileReader;
            _configurationTypes = new List<ConfigurationType>()
            {
                new ConfigurationType("solution:", IsSupportedSolutionFile, new HashSet<string>()),
                new ConfigurationType("project:", IsSupportedProjectFile, new HashSet<string>()),
                new ConfigurationType("file:", IsSupportedSourceFile, new HashSet<string>())
            };

            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            if (!_fileReader.FileExists(REPOSITORY_CONFIGURATION_FILE))
            {
                throw new FileNotFoundException("The repositories.config file could not be loaded.");
            }

            var lines = _fileReader.ReadAllLines(REPOSITORY_CONFIGURATION_FILE);

            foreach (var line in lines)
            {
                if (!line.StartsWith("//"))
                {
                    foreach (var configuration in _configurationTypes)
                    {
                        if (line.StartsWith(configuration.Separator))
                        {
                            var extension = line.Replace(configuration.Separator, "").Trim();

                            if (configuration.ValidationFunction(extension))
                            {
                                configuration.Extensions.Add(extension);
                            }
                            else
                            {
                                throw new ArgumentException($"The specified extension is not currently supported: {extension}. Implement it or remove it from the configuration file.");
                            }
                        }
                    }
                }
            }
        }

        public bool IsSolutionFile(string fileExtension)
        {
            const int SOLUTION = 0;
            return _configurationTypes[SOLUTION].Extensions.Contains(fileExtension);
        }

        public bool IsProjectFile(string fileExtension)
        {
            const int PROJECT = 1;
            return _configurationTypes[PROJECT].Extensions.Contains(fileExtension);
        }

        public bool IsSourceFile(string fileExtension)
        {
            const int FILE = 2;
            return _configurationTypes[FILE].Extensions.Contains(fileExtension);
        }

        private bool IsSupportedSolutionFile(string fileExtension)
        {
            return fileExtension == ".sln";
        }

        private bool IsSupportedProjectFile(string fileExtension)
        {
            return fileExtension == ".csproj" || fileExtension == ".vcxproj" || fileExtension == ".vcproj";
        }

        private bool IsSupportedSourceFile(string fileExtension)
        {
            return fileExtension == ".cs" || fileExtension == ".cpp" || fileExtension == ".h" || fileExtension == ".c";
        }
    }
}