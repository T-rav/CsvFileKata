using System.Collections.Generic;
using CsvFile.Kata.Dependencies;

namespace Csv.File
{
    public interface ICsvFileWriter
    {
        void Write(string fileName, List<Customer> customers);
    }
}