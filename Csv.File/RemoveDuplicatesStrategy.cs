using System.Collections.Generic;
using System.Linq;
using Csv.File.Domain;

namespace Csv.File
{
    public class RemoveDuplicatesStrategy : IDuplicationStrategy
    {
        public List<Customer> Apply(List<Customer> customers)
        {
            if(customers == null) return new List<Customer>();

            return customers.GroupBy(c => c.Name).Select(c => c.First()).ToList();
        }
    }
}