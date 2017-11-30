using System.Collections.Generic;
using CsvFile.Kata.Dependencies;

namespace Csv.File
{
    public interface ICsvBatchFileWriter
    {
        void WriteInBatchOf(string fileName, List<Customer> customer, int batchSize);
    }
}