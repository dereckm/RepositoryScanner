using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using RepositoryReaders.Directory;
using RepositoryReaders.Text;
using ILogger = Ninject.Extensions.Logging.ILogger;

namespace RepositoryScanComparer.ScanComparison
{
    public class ScanComparer
    {
        private readonly ILogger _logger;
        private readonly IFileReader _fileReader;
        private readonly IDirectoryReader _directoryReader;
        private readonly HashSet<string> _previousScan = new HashSet<string>();
        private readonly HashSet<string> _currentScan = new HashSet<string>();

        public ScanComparer(ILogger logger, IFileReader fileReader, IDirectoryReader directoryReader)
        {
            this._logger = logger;
            _fileReader = fileReader;
            _directoryReader = directoryReader;
        }

        public void CompareCurrentWithPrevious()
        {
            var files = _directoryReader.GetFiles(_directoryReader.GetCurrentDirectory(), "log_*.txt");

            if (files.Length < 2)
            {
                return;
            }

            var comparedFiles = (from file in files
                orderby DateTime.ParseExact(Path.GetFileNameWithoutExtension(file).Replace("log_", "")  ,
                    "yyyy_MM_d__HH_mm_ss", CultureInfo.InvariantCulture) descending
                select file).Take(2).ToList();

            var previous = comparedFiles[1];
            var current = comparedFiles[0];

            const string errorKey = "[ERR]";

            foreach (var line in _fileReader.ReadAllLines(previous))
            {
                if (line.Contains(errorKey))
                {
                    var indexOf = line.IndexOf(errorKey);
                    var trimmedLine = line.Remove(0, indexOf + 1 + errorKey.Length).Trim();
                    _previousScan.Add(trimmedLine);
                }
            }

            foreach (var line in _fileReader.ReadAllLines(current))
            {
                if (line.Contains(errorKey))
                {
                    var indexOf = line.IndexOf(errorKey);
                    var trimmedLine = line.Remove(0, indexOf + 1 + errorKey.Length).Trim();
                    _currentScan.Add(trimmedLine);
                }
            }

            _currentScan.ExceptWith(_previousScan);

            var totalProblems = 0;
            foreach (var line in _currentScan)
            {
               _logger.Error(line);
            }

            if (totalProblems > 0)
            {
                _logger.Info($"Total problems found: {totalProblems}.");
            }
        }
    }
}
