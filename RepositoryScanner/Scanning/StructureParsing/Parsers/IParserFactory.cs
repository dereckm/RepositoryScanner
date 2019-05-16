namespace RepositoryScanner.Scanning.StructureParsing.Parsers
{
    public interface IParserFactory<out T>
    {
        T CreateParser(string path);
    }
}
