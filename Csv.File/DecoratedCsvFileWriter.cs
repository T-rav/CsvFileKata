using System.Collections.Generic;

namespace Csv.File
{
    public class DecoratedCsvFileWriter : ICsvFileWriter
    {
        private readonly IFileSystem _fileSystem;

        public DecoratedCsvFileWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Write(string fileName, List<Customer> customers)
        {
            var productionWriter = new CsvFileWriter(_fileSystem, new RemoveDuplicatesStrategy());
            var debugWriter = new CsvFileWriter(_fileSystem, new NullDuplicatesStrategy());
            productionWriter.WriteInBatchOf(fileName, customers, 15000);
            debugWriter.WriteInBatchOf($"debug_{fileName}",customers, 20);
        }
    }
}