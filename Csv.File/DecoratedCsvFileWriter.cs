using System.Collections.Generic;
using CsvFile.Kata.Dependencies;

namespace Csv.File
{
    public class DecoratedCsvFileWriter : ICsvFileWriter
    {
        private readonly ICsvBatchFileWriter _productionWriter;
        private readonly ICsvBatchFileWriter _debugWriter;

        public DecoratedCsvFileWriter(IFileSystem fileSystem): 
            this(new CsvFileWriter(fileSystem, new RemoveDuplicatesStrategy()), 
                 new CsvFileWriter(fileSystem, new NullDuplicatesStrategy()))
        {
        }

        private DecoratedCsvFileWriter(ICsvBatchFileWriter productFileWriter, 
                                       ICsvBatchFileWriter debugFileWriter)
        {
            _productionWriter = productFileWriter;
            _debugWriter = debugFileWriter;
        }

        public void Write(string fileName, List<Customer> customers)
        {
            if (customers == null) return;
            if (string.IsNullOrWhiteSpace(fileName)) return;
            
            _productionWriter.WriteInBatchOf(fileName, customers, 15000);
            _debugWriter.WriteInBatchOf($"debug_{fileName}",customers, 20);
        }
    }
}