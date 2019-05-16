namespace RepositoryScanner.Scanning.StructureParsing.Parsers
{
    public interface IParser<out T>
    {
        T Parse(string path);
    }
}
