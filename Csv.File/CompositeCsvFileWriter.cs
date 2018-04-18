using System.Collections.Generic;
using CsvFile.Kata.Dependencies;

namespace Csv.File
{
    public class CompositeCsvFileWriter : ICsvFileWriter
    {
        private readonly ICsvBatchFileWriter _productionWriter;
        private readonly ICsvBatchFileWriter _debugWriter;

        public CompositeCsvFileWriter(IFileSystem fileSystem): 
            this(new BatchCsvFileWriter(fileSystem, new RemoveDuplicatesStrategy()), 
                 new BatchCsvFileWriter(fileSystem, new NullDuplicatesStrategy()))
        {
        }

        private CompositeCsvFileWriter(ICsvBatchFileWriter productFileWriter, 
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