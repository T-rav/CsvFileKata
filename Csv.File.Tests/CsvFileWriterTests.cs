﻿using System;
using System.Collections.Generic;
using CsvFile.Kata.Dependencies;
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
                var customer = new List<Customer> { new Customer { Name = "Bob Jones", ContactNumber = "0845009876" } };
                //---------------Act----------------
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
                var customer = CreateCustomers(5);
                //---------------Act----------------
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

            [TestCase("", TestName = "Empty string")]
            [TestCase(" ", TestName = "Whitespace")]
            [TestCase(null, TestName = "Null")]
            public void WhenNullOrWhitespaceFileName_ShouldNotWriteToFile(string fileName)
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
            [TestCase(11, 10, 1, TestName = "BatchSize of 10 with 11 records")]
            [TestCase(15001, 15000, 1, TestName = "BatchSize of 15000 with 15001 records")]
            [TestCase(15005, 15000, 5, TestName = "BatchSize of 15000 with 15005 records")]
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

            [Test]
            public void WhenBatchSizeEqualsNumberOfRecords_ShouldWriteToOneFile()
            {
                //---------------Arrange-------------------
                var totalRecords = 10;
                var batchSize = 10;
                var fileName = "import.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = CreateCsvFileWriter(fileSystem);
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

        private static CsvFileWriter CreateCsvFileWriter(IFileSystem fileSystem)
        {
            var duplicateStrategy = new RemoveDuplicatesStrategy();
            var writer = new CsvFileWriter(fileSystem, duplicateStrategy);
            return writer;
        }
    }
}
