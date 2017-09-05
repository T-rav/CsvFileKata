using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using PeanutButter.RandomGenerators;

namespace Csv.File.Tests
{
    [TestFixture]
    public class CsvFileWriterTests
    {
        [Test]
        public void Ctor_WhenNullFileSystem_ShouldThrowArgumentNullException()
        {
            //---------------Arrange-------------------
            //---------------Act----------------
            var result = Assert.Throws<ArgumentNullException>(() => new CsvFileWriter(null));
            //---------------Assert ----------------------
            Assert.AreEqual("fileSystem",result.ParamName);
        }

        [Test]
        public void WriteRecords_WhenSingleRecord_ShouldWriteToFile()
        {
            //---------------Arrange-------------------
            var fileName = "import.csv";
            var expectedLine = "Bob Jones,0845009876";
            var fileSystem = Substitute.For<IFileSystem>();
            var writer = new CsvFileWriter(fileSystem);
            //---------------Act----------------
            var customer = new List<Customer>{new Customer{Name = "Bob Jones", ContactNumber = "0845009876" }};
            writer.Write(fileName, customer);
            //---------------Assert ----------------------
            fileSystem.Received(1).WriteLine(fileName,expectedLine);
        }

        [Test]
        public void WriteRecords_WhenManyRecords_ShouldWriteAllToFile()
        {
            //---------------Arrange-------------------
            var fileName = "import.csv";
            var fileSystem = Substitute.For<IFileSystem>();
            var writer = new CsvFileWriter(fileSystem);
            //---------------Act----------------
            var customer = CreateCustomers(5);
            writer.Write(fileName, customer);
            //---------------Assert ----------------------
            fileSystem.Received(5).WriteLine(fileName, Arg.Any<string>());
        }

        [Test]
        public void WriteRecords_WhenNullCustomers_ShouldNotWriteToFile()
        {
            //---------------Arrange-------------------
            var fileName = "import.csv";
            var fileSystem = Substitute.For<IFileSystem>();
            var writer = new CsvFileWriter(fileSystem);
            //---------------Act----------------
            writer.Write(fileName, null);
            //---------------Assert ----------------------
            fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void WriteRecords_WhenEmptyOrNullFileName_ShouldNotWriteToFile(string fileName)
        {
            //---------------Arrange-------------------
            var fileSystem = Substitute.For<IFileSystem>();
            var writer = new CsvFileWriter(fileSystem);
            var customer = CreateCustomers(5);
            //---------------Act----------------
            writer.Write(fileName, customer);
            //---------------Assert ----------------------
            fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
        }

        [TestCase(11,10, 1)]
        [TestCase(15001, 15000,1)]
        [TestCase(15005, 15000, 5)]
        public void WriteInBatchOf_WhenJustMoreRecordsThanBatchSize_ShouldWriteToTwoFiles(int totalRecords, int batchSize, int remainderRecords)
        {
            //---------------Arrange-------------------
            var fileName = "import.csv";
            var fileSystem = Substitute.For<IFileSystem>();
            var writer = new CsvFileWriter(fileSystem);
            //---------------Act----------------
            var customer = CreateCustomers(totalRecords);
            writer.WriteInBatchOf(fileName, customer, batchSize);
            //---------------Assert ----------------------
            fileSystem.Received(batchSize).WriteLine($"1_{fileName}", Arg.Any<string>());
            fileSystem.Received(remainderRecords).WriteLine($"2_{fileName}", Arg.Any<string>());
        }

        [Test]
        public void WriteRecords_WhenDuplicateCustomers_ShouldRemoveDuplicateEntriesWhenWriting()
        {
            //---------------Arrange-------------------
            var fileName = "import.csv";
            var fileSystem = Substitute.For<IFileSystem>();
            var writer = new CsvFileWriter(fileSystem);
            var customers = CreateCustomers(10);
            //---------------Act----------------
            writer.Write(fileName, customers);
            //---------------Assert ----------------------
            fileSystem.Received(10).WriteLine(fileName, Arg.Any<string>());
        }

        private List<Customer> CreateCustomers(int customerCount)
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
