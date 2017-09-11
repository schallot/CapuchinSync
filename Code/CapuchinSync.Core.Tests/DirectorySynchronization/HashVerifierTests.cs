using System;
using System.IO;
using CapuchinSync.Core.DirectorySynchronization;
using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.DirectorySynchronization
{
    [TestFixture]
    public class HashVerifierTests
    {
        private IHashDictionaryEntry _entry;
        private IFileSystem _fileSystem;
        private IPathUtility _pathUtility;
        private IHashUtility _hashUtility;
        private string sourceRootDirectory;
        private string targetRootDirectory;
        private string fileRelativePath;
        private string hash;

        [SetUp]
        public void SetUp()
        {
            sourceRootDirectory = "C:\\source";
            targetRootDirectory = "D:\\target";
            fileRelativePath = "subDir\\file1.txt";
            hash = "123ABC";
            _hashUtility = Substitute.For<IHashUtility>();
            _hashUtility.HashName.Returns("testHash");
            _hashUtility.HashLength.Returns(hash.Length);
            _hashUtility.GetHashFromFile(Arg.Any<string>()).Returns(hash);
            _pathUtility = Substitute.For<IPathUtility>();
            _pathUtility.Combine(Arg.Any<string>(), Arg.Any<string>()).Returns(x => Path.Combine((string)x[0],(string)x[1]));
            _fileSystem = Substitute.For<IFileSystem>();
            _entry = Substitute.For<IHashDictionaryEntry>();
            _entry.RootDirectory.Returns(sourceRootDirectory);
            _entry.RelativePath.Returns(fileRelativePath);
            _entry.Hash.Returns(hash);
            _entry.IsValid.Returns(true);
        }

        [Test]
        public void Status_ShouldBeTargetFileDoesNotMatchHashForUnknownHashValues()
        {
            _entry.Hash.Returns(HashDictionaryEntry.UnknownHash);
            var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, _pathUtility, _hashUtility);
            Assert.AreEqual(HashVerifier.VerificationStatus.TargetFileDoesNotMatchHash, verifier.Status);
        }

        [Test]
        public void Status_ShouldBeTargetFileDoesNotExistWhenTargetFileIsMissing()
        {
            _fileSystem.DoesFileExist(Arg.Any<string>()).Returns(false);
            var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, _pathUtility, _hashUtility);
            Assert.AreEqual(HashVerifier.VerificationStatus.TargetFileDoesNotExist, verifier.Status);
        }

        [Test]
        public void Status_ShouldBeTargetFileNotReadWhenHashCalculationFails()
        {
            _fileSystem.DoesFileExist(Arg.Any<string>()).Returns(true);
            _hashUtility.GetHashFromFile(Arg.Any<string>()).Throws(x => new Exception("Mock failure to calculate hash"));
            var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, _pathUtility, _hashUtility);
            Assert.AreEqual(HashVerifier.VerificationStatus.TargetFileNotRead, verifier.Status);
        }

        [Test]
        public void CalculatedHash_ShouldComeFromHashUtility()
        {
            var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, _pathUtility, _hashUtility);
            Assert.AreEqual(hash.ToUpperInvariant(), verifier.CalculatedHash.ToUpperInvariant());
        }

        [Test]
        public void FullSourcePath_ShouldBeSet()
        {
            var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, _pathUtility, _hashUtility);
            Assert.AreEqual(Path.Combine(sourceRootDirectory, fileRelativePath), verifier.FullSourcePath);
        }

        [Test]
        public void FullTargetPath_ShouldBeSet()
        {
            var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, _pathUtility, _hashUtility);
            Assert.AreEqual(Path.Combine(targetRootDirectory, fileRelativePath), verifier.FullTargetPath);
        }

        [Test]
        public void RootSourceDirectory_ShouldBeSet()
        {
            var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, _pathUtility, _hashUtility);
            Assert.AreEqual(sourceRootDirectory, verifier.RootSourceDirectory);
        }

        [Test]
        public void RootTargetDirectory_ShouldBeSet()
        {
            var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, _pathUtility, _hashUtility);
            Assert.AreEqual(targetRootDirectory, verifier.RootTargetDirectory);
        }

        [Test]
        public void Constructor_NullEntryShouldResultInArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var verifier = new HashVerifier(null, targetRootDirectory, _fileSystem, _pathUtility, _hashUtility);
            });
        }

        [Test]
        public void Constructor_NullPathUtilityShouldResultInArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, null, _hashUtility);
            });
        }

        [Test]
        [TestCase(null,1)]
        [TestCase("",2)]
        [TestCase("    ",3)]
        [TestCase("\t \t",4)]
        public void Constructor_NullOrWhiteSpaceTargetDirectoryShouldResultInArgumentNullException(string targetDirectory, int testCaseId)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var verifier = new HashVerifier(_entry, targetDirectory, _fileSystem, _pathUtility, _hashUtility);
            }, $"Test case {testCaseId} should have thrown ArgumentNullException");
        }

        [Test]
        public void Constructor_NullHashUtilityShouldResultInArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var verifier = new HashVerifier(_entry, targetRootDirectory, _fileSystem, _pathUtility, null);
            });
        }

        [Test]
        [Ignore("Work in progress")]
        public void RecalculateFileMatchTest()
        {
            Assert.Fail();
        }
    }
}