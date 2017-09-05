using System;
using System.Collections.Generic;

namespace Csv.File
{
    public class CsvFileWriter
    {
        private readonly IFileSystem _fileSystem;

        public CsvFileWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
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