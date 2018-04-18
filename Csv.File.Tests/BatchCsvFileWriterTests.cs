using System;
using System.Collections.Generic;
using CsvFile.Kata.Dependencies;
using NSubstitute;
using NUnit.Framework;

namespace Csv.File.Tests
{
    [TestFixture]
    public class BatchCsvFileWriterTests
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
                var result = Assert.Throws<ArgumentNullException>(() => new BatchCsvFileWriter(null, duplicationStrategy));
                //---------------Assert ----------------------
                Assert.AreEqual("fileSystem", result.ParamName);
            }

            [Test]
            public void WhenNullDuplicationStrategy_ShouldThrowArgumentNullException()
            {
                //---------------Arrange-------------------
                var fileSystem = Substitute.For<IFileSystem>();
                //---------------Act----------------
                var result = Assert.Throws<ArgumentNullException>(() => new BatchCsvFileWriter(fileSystem, null));
                //---------------Assert ----------------------
                Assert.AreEqual("duplicateStrategy", result.ParamName);
            }
        }

        [TestFixture]
        public class Write
        {
            [Test]
            public void WhenNullCustomers_ShouldNotWriteToFile()
            {
                //---------------Arrange-------------------
                var fileName = "import.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateBatchCsvFileWriter(fileSystem);
                //---------------Act----------------
                writer.WriteInBatchOf(fileName, null, 10);
                //---------------Assert ----------------------
                fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
            }

            [TestCase("", TestName = "Empty string")]
            [TestCase(" ", TestName = "Whitespace")]
            [TestCase(null, TestName = "Null")]
            public void WhenNullOrWhitespaceFileName_ShouldNotWriteToFile(string fileName)
            {
                //---------------Arrange-------------------
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateBatchCsvFileWriter(fileSystem);
                var customer = CreateCustomers(5);
                //---------------Act----------------
                writer.WriteInBatchOf(fileName, customer, 10);
                //---------------Assert ----------------------
                fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
            }
            
            [Test]
            public void WhenDuplicateCustomers_ShouldRemoveDuplicateEntriesWhenWriting()
            {
                //---------------Arrange-------------------
                var fileName = "import.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateBatchCsvFileWriter(fileSystem);
                var customers = CreateCustomersWithDuplicates(10, 2);
                //---------------Act----------------
                writer.WriteInBatchOf(fileName, customers, 10);
                //---------------Assert ----------------------
                var expected = "1_import.csv";
                fileSystem.Received(9).WriteLine(expected, Arg.Any<string>());
            }
        }

        [TestFixture]
        public class WriteInBatchOf
        {
            [TestCase(11, 10, 1, TestName = "BatchSize of 10 with 11 records")]
            [TestCase(15001, 15000, 1, TestName = "BatchSize of 15000 with 15001 records")]
            [TestCase(15005, 15000, 5, TestName = "BatchSize of 15000 with 15005 records")]
            public void WhenFewMoreRecordsThanBatchSize_ShouldWriteToTwoFiles(int totalRecords, int batchSize, int remainderRecords)
            {
                //---------------Arrange-------------------
                var fileName = "import.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateBatchCsvFileWriter(fileSystem);
                //---------------Act----------------
                var customer = CreateCustomers(totalRecords);
                writer.WriteInBatchOf(fileName, customer, batchSize);
                //---------------Assert ----------------------
                fileSystem.Received(batchSize).WriteLine($"1_{fileName}", Arg.Any<string>());
                fileSystem.Received(remainderRecords).WriteLine($"2_{fileName}", Arg.Any<string>());
            }

            [Test]
            public void WhenBatchSizeEqualsNumberOfRecords_ShouldWriteToOneFile()
            {
                //---------------Arrange-------------------
                var totalRecords = 10;
                var batchSize = 10;
                var fileName = "import.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateBatchCsvFileWriter(fileSystem);
                //---------------Act----------------
                var customer = CreateCustomers(totalRecords);
                writer.WriteInBatchOf(fileName, customer, batchSize);
                //---------------Assert ----------------------
                fileSystem.Received(totalRecords).WriteLine($"1_{fileName}", Arg.Any<string>());
                fileSystem.DidNotReceive().WriteLine($"2_{fileName}", Arg.Any<string>());
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

        private static BatchCsvFileWriter CreateBatchCsvFileWriter(IFileSystem fileSystem)
        {
            var duplicateStrategy = new RemoveDuplicatesStrategy();
            var writer = new BatchCsvFileWriter(fileSystem, duplicateStrategy);
            return writer;
        }
    }
}
