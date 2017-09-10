using System;
using CapuchinSync.Core.DirectorySynchronization;
using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.DirectorySynchronization
{
    [TestFixture]
    public class HashVerifierTests
    {
        private HashDictionaryEntry _entry;
        private IFileSystem _fileSystem;
        private IPathUtility _pathUtility;
        private IHashUtility _hashUtility;
        private string rootDirectory = "C:\\temp";

        [SetUp]
        public void SetUp()
        {
          throw new NotImplementedException();  
            //_entry = new HashDictionaryEntry();
        }

        [Test]
        [Ignore("Work in progress")]
        public void Constructor_NullEntryShouldResultInArgumentNullException()
        {
            throw new NotImplementedException();
            //var verifier = new HashVerifier(null, rootDirectory,)
        }

        [Test]
        [Ignore("Work in progress")]
        public void RecalculateFileMatchTest()
        {
            Assert.Fail();
        }
    }
}