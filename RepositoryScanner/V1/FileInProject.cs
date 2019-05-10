using System.IO;

namespace RepositoryScanner.V1
{
    public class FileInProject
    {
        private string _filePath { get; set; }
        

        public string ProjectFilePath { get; set; }
        public string ProjectName { get; set; }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;//System.Uri.UnescapeDataString(value)???
                FileExists = File.Exists(value);
            }
        }
        
        public bool FileExists { get; private set; }
        public string FileName { get; set; }
        public string FileRepository { get; set; }
        public string ProjectRepository { get; set; }
    }
}