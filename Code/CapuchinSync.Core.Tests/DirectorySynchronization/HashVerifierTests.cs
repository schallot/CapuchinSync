using System;
using CapuchinSync.Core.DirectorySynchronization;
using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
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
        private string rootDirectory = "C:\\temp";

        [SetUp]
        public void SetUp()
        {
            _hashUtility = Substitute.For<IHashUtility>();
            _pathUtility = Substitute.For<IPathUtility>();
            _fileSystem = Substitute.For<IFileSystem>();
            _entry = Substitute.For<IHashDictionaryEntry>();
        }

        [Test]
        public void Constructor_NullEntryShouldResultInArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var verifier = new HashVerifier(null, rootDirectory, _fileSystem, _pathUtility, _hashUtility);
            });
        }

        [Test]
        public void Constructor_NullPathUtilityShouldResultInArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var verifier = new HashVerifier(_entry, rootDirectory, _fileSystem, null, _hashUtility);
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
                var verifier = new HashVerifier(_entry, rootDirectory, _fileSystem, _pathUtility, null);
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