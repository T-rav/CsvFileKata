using System.Collections.Generic;
using Csv.File.Domain;
using PeanutButter.RandomGenerators;

namespace Csv.File.Tests
{
    public class CustomerTestDataFactory
    {

        public List<Customer> CreateCustomersWithDuplicates(int numberOfRecords, int duplicateCount)
        {
            var customers = CreateCustomers(numberOfRecords - duplicateCount);
            for (var i = 0; i < duplicateCount; i++)
            {
                var customer = new Customer { Name = "Bob Jones", ContactNumber = "0824009370" };
                customers.Add(customer);
            }
            return customers;
        }

        public List<Customer> CreateCustomers(int customerCount)
        {
            var result = new List<Customer>();
            for (var count = 0; count < customerCount; count++)
            {
                var name = $"{RandomValueGen.GetRandomAlphaString(8, 12)} {RandomValueGen.GetRandomAlphaString(8, 12)}";
                var number = RandomValueGen.GetRandomNumericString(9, 9);
                var customer = new Customer { Name = name, ContactNumber = number };
                result.Add(customer);
            }
            return result;
        }
    }
}
