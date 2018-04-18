using System;
using System.Collections.Generic;
using System.Linq;
using CsvFile.Kata.Dependencies;

namespace Csv.File
{
    public class CsvFileWriter : ICsvFileWriter
    {
        private readonly IDuplicationStrategy _duplicateStrategy;
        private readonly IFileSystem _fileSystem;

        public CsvFileWriter(IFileSystem fileSystem, IDuplicationStrategy duplicateStrategy)
        {
            _duplicateStrategy = duplicateStrategy ?? throw new ArgumentNullException(nameof(duplicateStrategy)); ;
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public void Write(string fileName, List<Customer> customers)
        {
            if (customers == null) return;
            if (string.IsNullOrWhiteSpace(fileName)) return;

            var dedupCustomers = _duplicateStrategy.Apply(customers);

            foreach (var customer in dedupCustomers)
            {
                _fileSystem.WriteLine(fileName, customer.ToString());
            }
        }
    }
}