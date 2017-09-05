using System.Collections.Generic;

namespace Csv.File.Domain
{
    public interface IDuplicationStrategy
    {
        List<Customer> Apply(List<Customer> customers);
    }
}