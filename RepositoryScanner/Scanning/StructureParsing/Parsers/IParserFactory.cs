namespace RepositoryScanner.Scanning.StructureParsing.Parsers
{
    public interface IParserFactory<out T>
    {
        IParser<T> CreateParser(string path);
    }
}
