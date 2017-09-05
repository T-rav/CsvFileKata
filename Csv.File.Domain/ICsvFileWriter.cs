using System.Collections.Generic;

namespace Csv.File.Domain
{
    public interface ICsvFileWriter
    {
        void Write(string fileName, List<Customer> customers);
    }
}