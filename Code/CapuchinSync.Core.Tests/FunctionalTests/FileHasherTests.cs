using System;
using System.Linq;
using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.FunctionalTests
{
    public class FileHasherTests : FileTestBase
    {
        [Test]
        public void Constructor_ShouldHashFile()
        {
            var testFile = TestSet1.First();
            CreateFile(testFile);
            var hasher = new FileHasher(HashUtility, PathUtility, testFile.FilePath);
            Assert.AreEqual(testFile.Hash, hasher.Hash);
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullExceptionForNullHashUtility()
        {
            Assert.Throws<ArgumentNullException>(
                () => new FileHasher(null, PathUtility, TestSet1.First().FilePath));
        }

        [Test]
        public void Constructor_UnknownHashShouldBeCalculatedForFileThatCouldNotBeOpened()
        {
            var testFile = TestSet1.First();
            HashUtility = Substitute.For<IHashUtility>();
            HashUtility.HashName.Returns("TEST HASH");
            HashUtility.HashLength.Returns(40);
            var exMessage = "Kablamo";
            HashUtility.GetHashFromFile("").ThrowsForAnyArgs(new Exception(exMessage));

            var hasher = new FileHasher(HashUtility, PathUtility, testFile.FilePath);

            Assert.AreEqual(HashDictionaryEntry.UnknownHash, hasher.Hash, "Expected the Unknown Hash value for hour hash, since the file could not be read.");
            Assert.IsTrue(hasher.LogEntries.Any(x=>x.Severity == LogEntry.LogSeverity.Warning), "Expected at least one warning log entry to be generated, since the file could not be read.");
            Assert.IsTrue(hasher.LogEntries.Any(x=>x.Severity == LogEntry.LogSeverity.Warning && x.Message.Contains("Defaulting to unknown hash")), "Expected a warning about defaulting to unknown hash.");
        }

        [Test]
        public void DictionaryEntryString_ShouldReturnHashAndPath()
        {
            var testFile = TestSet1.First();
            CreateFile(testFile);
            var hasher = new FileHasher(HashUtility, PathUtility, testFile.FilePath);

            Assert.AreEqual($"{testFile.Hash}{HashDictionaryEntry.Delimiter}{testFile.RelativePath}", hasher.GetDictionaryEntryString(TestSourceFolder));
        }

    }
}
