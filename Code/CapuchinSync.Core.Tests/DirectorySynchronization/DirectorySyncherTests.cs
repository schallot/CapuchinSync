using System;
using CapuchinSync.Core.DirectorySynchronization;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.DirectorySynchronization
{
    [TestFixture]
    public class DirectorySyncherTests
    {
        private IFileSystem _fileSystem;
        private IPathUtility _pathUtility;
        private IFileCopierFactory _fileCopierFactory;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _pathUtility = Substitute.For<IPathUtility>();
            _fileCopierFactory = Substitute.For<IFileCopierFactory>();
        }

        [Test]
        public void SynchronizeTest()
        {
            Assert.Fail();
        }

        // ReSharper disable ObjectCreationAsStatement
        [Test]
        public void ConstructorTest_ShouldThrowArgumentNullExceptionWithNullFileSystem()
        {
            Assert.Throws<ArgumentNullException>(() => new DirectorySyncher(null, _pathUtility, _fileCopierFactory));
        }

        [Test]
        public void ConstructorTest_ShouldThrowArgumentNullExceptionWithNullPathUtility()
        {
            Assert.Throws<ArgumentNullException>(() => new DirectorySyncher(_fileSystem, null, _fileCopierFactory));
        }

        [Test]
        public void ConstructorTest_ShouldThrowArgumentNullExceptionWithFileCopierFactory()
        {
            Assert.Throws<ArgumentNullException>(() => new DirectorySyncher(_fileSystem, _pathUtility, null));
        }
        // ReSharper restore ObjectCreationAsStatement

    }
}