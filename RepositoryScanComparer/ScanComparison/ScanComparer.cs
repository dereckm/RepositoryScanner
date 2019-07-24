using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using RepositoryReaders.Directory;
using RepositoryReaders.Path;
using RepositoryReaders.Text;
using ILogger = Ninject.Extensions.Logging.ILogger;

namespace RepositoryScanComparer.ScanComparison
{
    public class ScanComparer
    {
        private readonly ILogger _logger;
        private readonly IFileReader _fileReader;
        private readonly IDirectoryReader _directoryReader;
        private readonly IPathReader _pathReader;
        private readonly HashSet<string> _previousScan = new HashSet<string>();
        private readonly HashSet<string> _currentScan = new HashSet<string>();

        public ScanComparer(ILogger logger, IFileReader fileReader, IDirectoryReader directoryReader, IPathReader pathReader)
        {
            this._logger = logger;
            _fileReader = fileReader;
            _directoryReader = directoryReader;
            _pathReader = pathReader;
        }

        public void CompareCurrentWithPrevious()
        {
            var files = _directoryReader.GetFiles(_directoryReader.GetCurrentDirectory(), "log_*.txt");

            if (files.Length < 2)
            {
                throw new ArgumentOutOfRangeException($"{nameof(files.Length)}={files.Length}", "Not enough log files in the the directory to do a comparison. Expecting 2 or more files.");
            }

            var mostRecentLog = (from file in files
                orderby DateTime.ParseExact(_pathReader.GetFileNameWithoutExtension(file).Replace("log_", ""),
                    "yyyy_MM_d__HH_mm_ss", CultureInfo.InvariantCulture) descending
                select file).Take(1).First();

            
            const string errorKey = "[ERR]";

            foreach (var file in files)
            {
                if (file == mostRecentLog)
                {
                    continue;
                }

                foreach (var line in _fileReader.ReadAllLines(file))
                {
                    if (line.Contains(errorKey))
                    {
                        var indexOf = line.IndexOf(errorKey, StringComparison.Ordinal);
                        var trimmedLine = line.Remove(0, indexOf + 1 + errorKey.Length).Trim();
                        _previousScan.Add(trimmedLine);
                    }
                }
            }

            foreach (var line in _fileReader.ReadAllLines(mostRecentLog))
            {
                if (line.Contains(errorKey))
                {
                    var indexOf = line.IndexOf(errorKey, StringComparison.Ordinal);
                    var trimmedLine = line.Remove(0, indexOf + 1 + errorKey.Length).Trim();
                    _currentScan.Add(trimmedLine);
                }
            }

            _currentScan.ExceptWith(_previousScan);

            var totalProblems = 0;
            foreach (var line in _currentScan)
            {
               _logger.Error(line);
               totalProblems++;
            }

            if (totalProblems > 0)
            {
                _logger.Info($"Total problems found: {totalProblems}.");
            }
        }
    }
}
