using NSubstitute;
using NUnit.Framework;

namespace Csv.File.Tests
{
    [TestFixture]
    public class DecoratedCsvFileWriterTests
    {
        [Test]
        public void WriteRecords_WhenNotNullCustomers_ShouldWriteToProductionAndDebugFiles()
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
    }
}
