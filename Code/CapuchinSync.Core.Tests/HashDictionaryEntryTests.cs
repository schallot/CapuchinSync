using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests
{
    [TestFixture]
    public class HashDictionaryEntryTests
    {
        private IHashUtility _hashUtility;

        [SetUp]
        public void SetUp()
        {
            _hashUtility = new Sha1Hash();
        }

        [Test]
        public void HashDictionaryEntryConstructor_ShouldCreateBasicHashDictionaryEntry()
        {
            var hash = "1234567890123456789012345678901234567890";
            var path = "somePath.txt";
            var hashLine = $"{hash} {path}";
            var directory = "C:\\Temp";
            var entry = new HashDictionaryEntry(_hashUtility, directory, hashLine);
            Assert.IsTrue(entry.IsValid, "Expected entry to be valid.");
            Assert.AreEqual(hash, entry.Hash, "Unexpected hash.");
            Assert.AreEqual(path, entry.RelativePath, "Unexpected relative path.");
            Assert.AreEqual(directory, entry.RootDirectory, "Unexpected root directory");
            Assert.IsTrue(string.IsNullOrWhiteSpace(entry.ErrorMessage), $"Expected error message to be empty, but was {entry.ErrorMessage}.");
        }

        [Test]
        public void HashDictionaryEntryConstructor_ShouldErrorOnShortLine()
        {
            var hashLine = "blah";
            var directory = "C:\\Temp";
            var entry = new HashDictionaryEntry(_hashUtility, directory, hashLine);
            Assert.IsFalse(entry.IsValid, "Expected entry to be invalid.");
            Assert.AreEqual($"Hash file line <{hashLine}> is not long enough to be valid.", entry.ErrorMessage, "Unexpected error message.");
        }

        [Test]
        public void HashDictionaryEntryConstructor_ShouldErrorOnBlankLine()
        {
            var hash = "                                        ";
            var path = "            ";
            var hashLine = $"{hash} {path}";
            var directory = "C:\\Temp";
            var entry = new HashDictionaryEntry(_hashUtility, directory, hashLine);
            Assert.IsFalse(entry.IsValid, "Expected entry to be invalid.");
            Assert.AreEqual($"Cannot process blank hash file line in hash file in {directory}", entry.ErrorMessage, "Unexpected error message.");
        }

        [Test]
        public void HashDictionaryEntryConstructor_ShouldErrorInvalidHashCharacter()
        {
            var hash = "12345678901234567890123456789012345#7890";
            var path = "whatever.txt";
            var hashLine = $"{hash} {path}";
            var directory = "C:\\Temp";
            var entry = new HashDictionaryEntry(_hashUtility, directory, hashLine);
            Assert.IsFalse(entry.IsValid, "Expected entry to be invalid.");
            Assert.AreEqual($"Hash <{hash}> in hash dictionary for folder {directory} contains non-hex character #.", entry.ErrorMessage, "Unexpected error message.");
        }
    }
}
