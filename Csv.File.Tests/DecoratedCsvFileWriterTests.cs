using CsvFile.Kata.Dependencies;
using NSubstitute;
using NUnit.Framework;

namespace Csv.File.Tests
{
    [TestFixture]
    public class DecoratedCsvFileWriterTests
    {
        public class Write
        {
            [Test]
            public void WhenNotNullCustomers_ShouldWriteToProductionAndDebugFiles()
            {
                //---------------Arrange-------------------
                var productionBatchSize = 15000;
                var debugBatchSize = 20;
                var numberOfRecords = 15003;
                var fileName = "file.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = new DecoratedCsvFileWriter(fileSystem);
                var customers = new CustomerTestDataFactory().CreateCustomersWithDuplicates(numberOfRecords, 2);
                //---------------Act----------------
                writer.Write(fileName, customers);
                //---------------Assert ----------------------
                fileSystem.Received(productionBatchSize).WriteLine("1_file.csv", Arg.Any<string>());
                fileSystem.Received(2).WriteLine("2_file.csv", Arg.Any<string>());
                fileSystem.Received(debugBatchSize).WriteLine("1_debug_file.csv", Arg.Any<string>());
                fileSystem.Received(3).WriteLine("751_debug_file.csv", Arg.Any<string>());
            }

            [Test]
            public void WhenNullCustomers_ShouldNotWriteToProductionAndDebugFiles()
            {
                //---------------Arrange-------------------
                var fileName = "file.csv";
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = new DecoratedCsvFileWriter(fileSystem);
                //---------------Act----------------
                writer.Write(fileName, null);
                //---------------Assert ----------------------
                fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
                fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
            }

            [TestCase("", TestName = "Empty String")]
            [TestCase(" ", TestName = "Whitespace")]
            [TestCase(null, TestName = "Null")]
            public void WhenNullOrWhiteSpaceFilename_ShouldNotWriteToProductionAndDebugFiles(string fileName)
            {
                //---------------Arrange-------------------
                var fileSystem = Substitute.For<IFileSystem>();
                var writer = new DecoratedCsvFileWriter(fileSystem);
                var customers = new CustomerTestDataFactory().CreateCustomers(5);
                //---------------Act----------------
                writer.Write(fileName, customers);
                //---------------Assert ----------------------
                fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
                fileSystem.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<string>());
            }
        }
        
    }
}
