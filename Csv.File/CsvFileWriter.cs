using System;
using System.Collections.Generic;
using System.Linq;

namespace Csv.File
{
    public class CsvFileWriter : ICsvFileWriter
    {
        private readonly IFileSystem _fileSystem;

        public CsvFileWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public void WriteInBatchOf(string fileName, List<Customer> customer, int batchSize)
        {
            var fileCount = 1;
            var totalFiles = Math.Ceiling(customer.Count / (double)batchSize);
            
            while (fileCount <= totalFiles)
            {
                var batch = GetBatch(customer, batchSize, fileCount-1);
                var batchFileName = GetBatchFileName(fileName, fileCount);
                Write(batchFileName, batch.ToList());
                fileCount++;
            }
        }

        private static string GetBatchFileName(string fileName, int fileCount)
        {
            var batchFileName = $"{fileCount}_{fileName}";
            return batchFileName;
        }

        private static IEnumerable<Customer> GetBatch(List<Customer> customer, int batchSize, int fileCount)
        {
            var batchedRecordsTaken = batchSize * fileCount;
            var batch = customer.Skip(batchedRecordsTaken).Take(batchSize);
            return batch;
        }

        public void Write(string fileName, List<Customer> customers)
        {
            if (customers == null) return;
            if (string.IsNullOrWhiteSpace(fileName)) return;

            foreach (var customer in customers)
            {
                var line = string.Join(",", customer.Name, customer.ContactNumber);
                _fileSystem.WriteLine(fileName, line);
            }
        }

    }
}