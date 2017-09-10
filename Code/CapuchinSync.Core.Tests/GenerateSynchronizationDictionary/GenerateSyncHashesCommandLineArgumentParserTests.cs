using System;
using CapuchinSync.Core.GenerateSynchronizationDictionary;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.GenerateSynchronizationDictionary
{
    [TestFixture]
    public class GenerateSyncHashesCommandLineArgumentParserTests
    {
        private IFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.DoesDirectoryExist(Arg.Any<string>()).Returns(true);
        }

        [Test]
        public void Constructor_EverythingShouldBePeachy()
        {
            const string rootDir = "blah";
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] {rootDir}, _fileSystem);

            Assert.AreEqual(Constants.EverythingsJustPeachyReturnCode, parser.ErrorNumber, "Expected everything to be peachy");
        }

        [Test]
        public void Constructor_RootDirectoryShouldBeParsedFromFirstArgument()
        {
            const string rootDir = "blah";
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { rootDir }, _fileSystem);

            Assert.IsNotNull(parser.Arguments, "Expected arguments to be non-null");
            Assert.AreEqual(rootDir, parser.Arguments.RootDirectory, "Unexpected root directory");
        }

        [Test]
        public void Constructor_ExtensionsToExclude()
        {
            throw new NotImplementedException();
        }
    }
}