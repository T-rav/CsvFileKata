using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Csv.File.Tests
{
    [TestFixture]
    public class RemoveDuplicatesStrategyTests
    {
        [Test]
        public void Apply_WhenDuplicateCustomers_ShouldRemoveDuplicates()
        {
            //---------------Arrange-------------------
            var expected = 11;
            var totalRecords = 12;
            var duplicates = 2;
            var removeDuplicatesStrategy = new RemoveDuplicatesStrategy();
            var customers = new CustomerTestDataFactory().CreateCustomersWithDuplicates(totalRecords,duplicates);
            //---------------Act----------------
            var result = removeDuplicatesStrategy.Apply(customers);
            //---------------Assert ----------------------
            Assert.AreEqual(expected, result.Count);
        }

        [Test]
        public void Apply_WhenNoDuplicateCustomers_ShouldRemoveDuplicates()
        {
            //---------------Arrange-------------------
            var expected = 12;
            var totalRecords = 12;
            var removeDuplicatesStrategy = new RemoveDuplicatesStrategy();
            var customers = new CustomerTestDataFactory().CreateCustomers(totalRecords);
            //---------------Act----------------
            var result = removeDuplicatesStrategy.Apply(customers);
            //---------------Assert ----------------------
            Assert.AreEqual(expected, result.Count);
        }

        [Test]
        public void Apply_WhenNullCustomers_ShouldReturnEmptyList()
        {
            //---------------Arrange-------------------
            var removeDuplicatesStrategy = new RemoveDuplicatesStrategy();
            //---------------Act----------------
            var result = removeDuplicatesStrategy.Apply(null);
            //---------------Assert ----------------------
            CollectionAssert.IsEmpty(result);
        }
    }

    public class RemoveDuplicatesStrategy : IDuplicationStrategy
    {
        public List<Customer> Apply(List<Customer> customers)
        {
            if(customers == null) return new List<Customer>();

            return customers.GroupBy(c => c.Name).Select(c => c.First()).ToList();
        }
    }
}