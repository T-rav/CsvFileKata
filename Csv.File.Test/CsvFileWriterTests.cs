using System;
using NUnit.Framework;

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
    }

    public class CsvFileWriter
    {
        private IFileSystem _fileSystem;

        public CsvFileWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }
    }
}
