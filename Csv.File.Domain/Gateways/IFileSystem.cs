namespace Csv.File.Domain.Gateways
{
    public interface IFileSystem
    {
        void WriteLine(string fileName, string line);
    }
}
