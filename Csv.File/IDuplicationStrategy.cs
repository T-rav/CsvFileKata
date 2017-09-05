using System.Collections.Generic;

namespace Csv.File
{
    public interface IDuplicationStrategy
    {
        List<Customer> Apply(List<Customer> customers);
    }
}