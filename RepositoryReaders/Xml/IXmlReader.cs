namespace RepositoryReaders.Xml
{
    public interface IXmlReader
    {
        void Create(string path);
        bool ReadToFollowing(string name);
        string GetAttribute(string name);
    }
}
