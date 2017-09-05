using System.Collections.Generic;

namespace Csv.File
{
    public interface ICsvFileWriter
    {
        void Write(string fileName, List<Customer> customers);
        void WriteInBatchOf(string fileName, List<Customer> customer, int batchSize);
    }
}