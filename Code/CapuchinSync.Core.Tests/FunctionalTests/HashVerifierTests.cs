using System.IO;
using System.Linq;
using CapuchinSync.Core.DirectorySynchronization;
using CapuchinSync.Core.Hashes;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.FunctionalTests
{
    public class HashVerifierTests : FileTestBase
    {
        [Test]
        public void ShouldVerifyWithValidHash()
        {
            var testFile = TestSet1.First();
            testFile.RootDirectory = TestDestinationFolder;
            CreateFile(testFile);
            var entry = new HashDictionaryEntry(HashUtility, TestSourceFolder, testFile.HashFileLine);
            // It shouldn't matter if the source file exists or not, as long as the destination file's hash verifies.
            var verifier = new HashVerifier(entry, TestDestinationFolder, FileSystem, PathUtility, HashUtility);
            Assert.AreEqual(testFile.Hash, verifier.GetHashEntry().Hash, $"File at {testFile.FilePath} has a different hash than what the verifier determined.");
            Assert.AreEqual(verifier.Status, HashVerifier.VerificationStatus.TargetFileMatchesHash, "Expected calculated hash to match the hash line.");
        }

        [Test]
        public void ShouldFailVerificationWithInvalidValidHash()
        {
            var testFile = TestSet1.First();
            testFile.RootDirectory = TestDestinationFolder;
            testFile.Contents += "\r\nThis extra text should invalidate the hash.";
            CreateFile(testFile);
            var entry = new HashDictionaryEntry(HashUtility, TestSourceFolder, testFile.HashFileLine);
            // It shouldn't matter if the source file exists or not, as long as the destination file's hash verifies.
            var verifier = new HashVerifier(entry, TestDestinationFolder, FileSystem, PathUtility, HashUtility);
            Assert.AreNotEqual(testFile.Hash, verifier.CalculatedHash, $"File at {testFile.FilePath} should have a different hash than what the verifier determined.");
            Assert.AreEqual(verifier.Status, HashVerifier.VerificationStatus.TargetFileDoesNotMatchHash, "Expected calculated hash to differ from the hash line.");
        }


        [Test]
        public void ShouldFailToReadFileThatDoesNotExist()
        {
            var testFile = TestSet1.First();
            testFile.RootDirectory = TestDestinationFolder;
            Assert.IsFalse(File.Exists(testFile.FilePath), "Setup error: Test file is not supposed to exist.");
            //CreateFile(testFile); // We won't bother to create the file this time.
            var entry = new HashDictionaryEntry(HashUtility, TestSourceFolder, testFile.HashFileLine);
            var verifier = new HashVerifier(entry, TestDestinationFolder, FileSystem, PathUtility, HashUtility);
            Assert.AreEqual(verifier.Status, HashVerifier.VerificationStatus.TargetFileDoesNotExist, "Expected VerificationStatus to indicate that the file does not exist.");
        }

        [Test]
        public void ExpectedHashShouldMatchHashFromSourceLine()
        {
            var testFile = TestSet1.First();
            testFile.RootDirectory = TestDestinationFolder;
            // It shouldn't matter if the source file exists or not, as long as the destination file's hash verifies.
            var entry = new HashDictionaryEntry(HashUtility, TestSourceFolder, testFile.HashFileLine);
            var verifier = new HashVerifier(entry, TestDestinationFolder, FileSystem, PathUtility, HashUtility);
            Assert.AreEqual(testFile.Hash, verifier.GetHashEntry().Hash, "Unexpected ExpectedHash value");
        }

        // TODO: Test failed file read.
    }
}
