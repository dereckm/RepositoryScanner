using System.Collections;
using System.Collections.Generic;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public class RepositoryRegistryBase : IRepositoryRegistry
    {
        protected readonly List<Repository> Registry = new List<Repository>();
        private IEnumerator<Repository> _currentEnumerator;

        public Repository Current => _currentEnumerator.Current;

        public Repository GetRepositoryFromPath(string path)
        {
            foreach (var repository in Registry)
            {
                var repositoryPath = repository.Path;

                if (path.ToLower().StartsWith(repositoryPath.ToLower()))
                {
                    return new Repository(repositoryPath);
                }
            }

            return null;
        }

        public IEnumerator<Repository> GetEnumerator()
        {
            _currentEnumerator = Registry.GetEnumerator();
            return _currentEnumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}