using System;
using System.Collections.Generic;
using System.Linq;
using Csv.File.Domain;

namespace Csv.File.Tests
{
    public class CustomerTestDataFactory
    {
        private static readonly Random random = new Random();

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
                var name = $"{RandomString(6)} {RandomString(12)}";
                var number = RandomNumber(9);
                var customer = new Customer { Name = name, ContactNumber = number };
                result.Add(customer);
            }
            return result;
        }

        
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcedfghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string RandomNumber(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
