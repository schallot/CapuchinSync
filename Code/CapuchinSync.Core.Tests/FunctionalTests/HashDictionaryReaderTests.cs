using System.IO;
using System.Linq;
using CapuchinSync.Core.Hashes;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.FunctionalTests
{
    public class HashDictionaryReaderTests : FileTestBase
    {
        [Test]
        public void ReadHashDictionary_ShouldFindTwoHashes()
        {
            var dictionaryContents =
                @"2 files
1234567890123456789012345678901234567890{delimiter}somePath.txt
1234567890123456789012345678901234567891{delimiter}subdir\somePath2.txt
".Replace("{delimiter}", HashDictionaryEntry.Delimiter);
            CreateFile(Constants.HashFileName, dictionaryContents);
            var reader = new HashDictionaryReader(HashUtility,TestSourceFolder, FileSystem, PathUtility);
            var hashDictionary = reader.Read();
            Assert.AreEqual(0, reader.ErrorCode, "Unexpected error code.");
            Assert.AreEqual(Path.Combine(TestSourceFolder,Constants.HashFileName), hashDictionary.FilePath, "Unexpected file path.");
            Assert.AreEqual(2, hashDictionary.Entries.Count, "Expected 2 hash entries.");

            var hash1 = hashDictionary.Entries.FirstOrDefault(x => x.Hash == "1234567890123456789012345678901234567890");
            var hash2 = hashDictionary.Entries.FirstOrDefault(x => x.Hash == "1234567890123456789012345678901234567891");

            Assert.IsNotNull(hash1, "First hash was not found");
            Assert.IsNotNull(hash2, "Second hash was not found");

            Assert.IsTrue(hash1.IsValid, "The first hash should have been considered valid");
            Assert.IsTrue(hash2.IsValid, "The second hash should have been considered valid");

            Assert.AreEqual("somePath.txt",hash1.RelativePath, "Unexpected RelativePath in first hash.");
            Assert.AreEqual("subdir\\somePath2.txt", hash2.RelativePath, "Unexpected RelativePath in second hash.");

            Assert.IsTrue(string.IsNullOrWhiteSpace(hash1.ErrorMessage), "Unexpected error message in first hash");
            Assert.IsTrue(string.IsNullOrWhiteSpace(hash2.ErrorMessage), "Unexpected error message in second hash");

            Assert.AreEqual(TestSourceFolder, hash1.RootDirectory, "Unexpected root directory for first hash");
            Assert.AreEqual(TestSourceFolder, hash2.RootDirectory, "Unexpected root directory for second hash");
        }

        [Test]
        public void ReadHashDictionary_ShouldStripOffLeadingSlashFromPath()
        {
            var dictionaryContents =
                @"1 files
1234567890123456789012345678901234557890{delimiter}/somePath.txt
".Replace("{delimiter}", HashDictionaryEntry.Delimiter);
            CreateFile(Constants.HashFileName, dictionaryContents);
            var reader = new HashDictionaryReader(HashUtility, TestSourceFolder, FileSystem, PathUtility);
            var dictionary = reader.Read();
            Assert.AreEqual(0, reader.ErrorCode, "Unexpected error code.");
            Assert.AreEqual("somePath.txt", dictionary.Entries.Single().RelativePath, "Unexpected relative path.");
        }

        [Test]
        public void ReadHashDictionary_ShouldStripOutBlankLines()
        {
            var dictionaryContents =
                @"1 files

1234567890123456789012345678901234557890{delimiter}/somePath.txt

".Replace("{delimiter}", HashDictionaryEntry.Delimiter);
            CreateFile(Constants.HashFileName, dictionaryContents);
            var reader = new HashDictionaryReader(HashUtility, TestSourceFolder, FileSystem, PathUtility);
            var dictionary = reader.Read();
            Assert.AreEqual(0, reader.ErrorCode, "Unexpected error code.");
            Assert.AreEqual(1, dictionary.Entries.Count, "Unexpected number of entries read.");
        }


        [Test]
        public void ReadHashDictionary_ShouldGiveInvalidHashError()
        {
            var dictionaryContents =
                @"1 files
12345678901234567890123456789012345Z7890{delimiter}somePath.txt
".Replace("{delimiter}", HashDictionaryEntry.Delimiter);
            CreateFile(Constants.HashFileName, dictionaryContents);
            var reader = new HashDictionaryReader(HashUtility, TestSourceFolder, FileSystem, PathUtility);
            reader.Read();
            Assert.AreEqual(HashDictionaryReader.ErrorCodes.InvalidHashDictionaryLine, reader.ErrorCode, "Unexpected error code.");
        }

        [Test]
        public void ReadHashDictionary_ShouldGiveCouldNotReadFileCountFromFirstLineError()
        {
            var dictionaryContents =
                @"blah files
12345678901234567890123456789012345Z7890{delimiter}somePath.txt
".Replace("{delimiter}", HashDictionaryEntry.Delimiter);
            CreateFile(Constants.HashFileName, dictionaryContents);
            var reader = new HashDictionaryReader(HashUtility, TestSourceFolder, FileSystem, PathUtility);
            reader.Read();
            Assert.AreEqual(HashDictionaryReader.ErrorCodes.CouldNotReadFileCountFromFirstLine, reader.ErrorCode, "Unexpected error code.");
        }

        [Test]
        public void ReadHashDictionary_ShouldGiveUnexpectedHashCountError()
        {
            var dictionaryContents =
                @"88 files
1234567890123456789012345678901234537890{delimiter}somePath.txt
".Replace("{delimiter}", HashDictionaryEntry.Delimiter);
            CreateFile(Constants.HashFileName, dictionaryContents);
            var reader = new HashDictionaryReader(HashUtility, TestSourceFolder, FileSystem, PathUtility);
            reader.Read();
            Assert.AreEqual(HashDictionaryReader.ErrorCodes.UnexpectedNumberOfHashesInFile, reader.ErrorCode, "Unexpected error code.");
        }


        [Test]
        public void ReadHashDictionary_ShouldGiveInvalidHashDictionaryError()
        {
            var dictionaryContents =
                @"1 files
3838383838 invalid line
";
            CreateFile(Constants.HashFileName, dictionaryContents);
            var reader = new HashDictionaryReader(HashUtility, TestSourceFolder, FileSystem, PathUtility);
            reader.Read();
            Assert.AreEqual(HashDictionaryReader.ErrorCodes.InvalidHashDictionaryLine, reader.ErrorCode, "Unexpected error code.");
        }


        [Test]
        public void ReadHashDictionary_ShouldGiveHashDictionaryNotFoundError()
        {
            // We won't bother creating the hash file
            var reader = new HashDictionaryReader(HashUtility, TestSourceFolder, FileSystem, PathUtility);
            reader.Read();
            Assert.AreEqual(HashDictionaryReader.ErrorCodes.HashDictionaryNotFound, reader.ErrorCode, "Unexpected error code.");
        }
    }
}
