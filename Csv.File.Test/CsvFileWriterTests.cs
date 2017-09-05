using System;
using System.Collections.Generic;
using Csv.File.Domain;
using Csv.File.Domain.Gateways;
using NSubstitute;
using NUnit.Framework;

namespace Csv.File.Tests
{
    [TestFixture]
    public class CsvFileWriterTests
    {
        [TestFixture]
        public class Constuctor
        {
            [Test]
            public void WhenNullFileSystem_ShouldThrowArgumentNullException()
            {
                //---------------Arrange-------------------
                var duplicationStrategy = Substitute.For<IDuplicationStrategy>();
                //---------------Act----------------
                var result = Assert.Throws<ArgumentNullException>(() => new CsvFileWriter(null, duplicationStrategy));
                //---------------Assert ----------------------
                Assert.AreEqual("fileSystem", result.ParamName);
            }

            [Test]
            public void WhenNullDuplicationStrategy_ShouldThrowArgumentNullException()
            {
                //---------------Arrange-------------------
                var fileSystem = Substitute.For<IFileSystem>();
                //---------------Act----------------
                var result = Assert.Throws<ArgumentNullException>(() => new CsvFileWriter(fileSystem, null));
                //---------------Assert ----------------------
                Assert.AreEqual("duplicateStrategy", result.ParamName);
            }
        }

        [TestFixture]
        public class Write
        {
            [Test]
            public void WhenSingleRecord_ShouldWriteToFile()
            {
                //---------------Arrange-------------------
                var fileName = "import.csv";
                var expectedLine = "Bob Jones,0845009876";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateCsvFileWriter(fileSystem);
                //---------------Act----------------
                var customer = new List<Customer> { new Customer { Name = "Bob Jones", ContactNumber = "0845009876" } };
                writer.Write(fileName, customer);
                //---------------Assert ----------------------
                fileSystem.Received(1).WriteLine(fileName, expectedLine);
            }

            [Test]
            public void WhenManyRecords_ShouldWriteAllToFile()
            {
                //---------------Arrange-------------------
                var fileName = "import.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateCsvFileWriter(fileSystem);
                //---------------Act----------------
                var customer = CreateCustomers(5);
                writer.Write(fileName, customer);
                //---------------Assert ----------------------
                fileSystem.Received(5).WriteLine(fileName, Arg.Any<string>());
            }

            [Test]
            public void WhenNullCustomers_ShouldNotWriteToFile()
            {
                //---------------Arrange-------------------
                var fileName = "import.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateCsvFileWriter(fileSystem);
                //---------------Act----------------
                writer.Write(fileName, null);
                //---------------Assert ----------------------
                fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
            }

            [TestCase("")]
            [TestCase(" ")]
            [TestCase(null)]
            public void WhenEmptyOrNullFileName_ShouldNotWriteToFile(string fileName)
            {
                //---------------Arrange-------------------
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateCsvFileWriter(fileSystem);
                var customer = CreateCustomers(5);
                //---------------Act----------------
                writer.Write(fileName, customer);
                //---------------Assert ----------------------
                fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
            }


            [Test]
            public void WhenDuplicateCustomers_ShouldRemoveDuplicateEntriesWhenWriting()
            {
                //---------------Arrange-------------------
                var fileName = "import.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateCsvFileWriter(fileSystem);
                var customers = CreateCustomersWithDuplicates(10, 2);
                //---------------Act----------------
                writer.Write(fileName, customers);
                //---------------Assert ----------------------
                fileSystem.Received(9).WriteLine(fileName, Arg.Any<string>());
            }
        }

        [TestFixture]
        public class WriteInBatchOf
        {
            [TestCase(11, 10, 1)]
            [TestCase(15001, 15000, 1)]
            [TestCase(15005, 15000, 5)]
            public void WhenFewMoreRecordsThanBatchSize_ShouldWriteToTwoFiles(int totalRecords, int batchSize, int remainderRecords)
            {
                //---------------Arrange-------------------
                var fileName = "import.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateCsvFileWriter(fileSystem);
                //---------------Act----------------
                var customer = CreateCustomers(totalRecords);
                writer.WriteInBatchOf(fileName, customer, batchSize);
                //---------------Assert ----------------------
                fileSystem.Received(batchSize).WriteLine($"1_{fileName}", Arg.Any<string>());
                fileSystem.Received(remainderRecords).WriteLine($"2_{fileName}", Arg.Any<string>());
            }
        }

        private static List<Customer> CreateCustomersWithDuplicates(int numberOfRecords, int duplicateCount)
        {
            return new CustomerTestDataFactory().CreateCustomersWithDuplicates(numberOfRecords, duplicateCount);
        }

        private static List<Customer> CreateCustomers(int totalRecords)
        {
            return new CustomerTestDataFactory().CreateCustomers(totalRecords);
        }

        private static CsvFileWriter CreateCsvFileWriter(IFileSystem fileSystem)
        {
            var duplicateStrategy = new RemoveDuplicatesStrategy();
            var writer = new CsvFileWriter(fileSystem, duplicateStrategy);
            return writer;
        }
    }
}
