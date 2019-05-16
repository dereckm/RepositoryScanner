using System.Collections.Generic;
using RepositoryScanner.Scanning.Structure;

namespace RepositoryScanner.Scanning.FileExplorer
{
    public class RepositoryRegistryBase : IRepositoryRegistry
    {
        protected readonly List<Repository> Registry = new List<Repository>();
        private int _index = -1;

        public Repository Current
        {
            get => (Repository)Registry[_index].Clone();
        }

        public bool TryGetNext(out Repository repository)
        {
            if (_index < Registry.Count - 1)
            {
                _index++;
                repository = Current;
                return true;
            }

            repository = null;
            return false;
        }

        public Repository GetRepositoryFromPath(string path)
        {
            foreach (var repository in Registry)
            {
                var repositoryPath = repository.Path;

                if (path.StartsWith(repositoryPath))
                {
                    return new Repository(repositoryPath);
                }
            }

            return null;
        }
    }
}