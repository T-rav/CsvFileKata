using System.Collections.Generic;
using CsvFile.Kata.Dependencies;

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
            if (customers == null) return;
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var productionWriter = new CsvFileWriter(_fileSystem, new RemoveDuplicatesStrategy());
            var debugWriter = new CsvFileWriter(_fileSystem, new NullDuplicatesStrategy());
            productionWriter.WriteInBatchOf(fileName, customers, 15000);
            debugWriter.WriteInBatchOf($"debug_{fileName}",customers, 20);
        }
    }
}