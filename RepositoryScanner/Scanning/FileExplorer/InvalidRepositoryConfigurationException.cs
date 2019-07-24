using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public class InvalidRepositoryConfigurationException : Exception
    {
        public InvalidRepositoryConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}
