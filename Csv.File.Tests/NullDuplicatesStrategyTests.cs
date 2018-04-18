using NUnit.Framework;

namespace Csv.File.Tests
{
    [TestFixture]
    public class NullDuplicatesStrategyTests
    {
        [TestFixture]
        public class Apply
        {
            [Test]
            public void Apply_WhenDuplicateCustomers_ShouldReturnOriginalInput()
            {
                //---------------Arrange-------------------
                var expected = 12;
                var numberOfRecords = 12;
                var strategy = new NullDuplicatesStrategy();
                var duplicateCount = 2;
                var customers = new CustomerTestDataFactory()
                                   .CreateCustomersWithDuplicates(numberOfRecords, duplicateCount);
                //---------------Act----------------
                var result = strategy.Apply(customers);
                //---------------Assert ----------------------
                Assert.AreEqual(expected, result.Count);
                Assert.AreEqual(result, customers);
            }

            [Test]
            public void Apply_WhenNullCustomers_ShouldReturnEmptyList()
            {
                //---------------Arrange-------------------
                var strategy = new NullDuplicatesStrategy();
                //---------------Act----------------
                var result = strategy.Apply(null);
                //---------------Assert ----------------------
                CollectionAssert.IsEmpty(result);
            }
        }
    }
}