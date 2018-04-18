using System;
using System.Collections.Generic;
using System.Linq;
using CsvFile.Kata.Dependencies;

namespace Csv.File
{
    public class BatchCsvFileWriter : ICsvBatchFileWriter
    {
        private readonly IDuplicationStrategy _duplicateStrategy;
        private readonly IFileSystem _fileSystem;

        public BatchCsvFileWriter(IFileSystem fileSystem, IDuplicationStrategy duplicateStrategy)
        {
            _duplicateStrategy = duplicateStrategy ?? throw new ArgumentNullException(nameof(duplicateStrategy)); ;
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public void WriteInBatchOf(string fileName, List<Customer> customers, int batchSize)
        {
            if (customers == null) return;
            if (string.IsNullOrWhiteSpace(fileName)) return;

            var fileCount = 1;
            var totalFiles = Math.Ceiling(customers.Count / (double)batchSize);

            var dedupCustomers = DedupCustomers(customers);

            while (fileCount <= totalFiles)
            {
                var batch = GetBatch(dedupCustomers, batchSize, fileCount - 1);
                var batchFileName = GetBatchFileName(fileName, fileCount);
                Write(batchFileName, batch.ToList());
                fileCount++;
            }
        }

        private List<Customer> DedupCustomers(List<Customer> customers)
        {
            var dedupCustomers = _duplicateStrategy.Apply(customers);
            return dedupCustomers;
        }

        private void Write(string fileName, List<Customer> customers)
        {
            foreach (var customer in customers)
            {
                _fileSystem.WriteLine(fileName, customer.ToString());
            }
        }

        private string GetBatchFileName(string fileName, int fileCount)
        {
            var batchFileName = $"{fileCount}_{fileName}";
            return batchFileName;
        }

        private IEnumerable<Customer> GetBatch(List<Customer> customer, int batchSize, int fileCount)
        {
            var batchedRecordsTaken = batchSize * fileCount;
            var batch = customer.Skip(batchedRecordsTaken).Take(batchSize);
            return batch;
        }
    }
}