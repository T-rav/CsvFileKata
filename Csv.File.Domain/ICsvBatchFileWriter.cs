using System.Collections.Generic;

namespace Csv.File.Domain
{
    public interface ICsvBatchFileWriter
    {
        void WriteInBatchOf(string fileName, List<Customer> customer, int batchSize);
    }
}