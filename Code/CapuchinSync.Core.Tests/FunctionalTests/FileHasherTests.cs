using System;
using System.Linq;
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
            var hasher = new FileHasher(HashUtility, TestSourceFolder, testFile.FilePath);
            Assert.AreEqual(testFile.Hash, hasher.Hash);
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullExceptionForNullHashUtility()
        {
            Assert.Throws<ArgumentNullException>(
                () => new FileHasher(null, TestSourceFolder, TestSet1.First().FilePath));
        }

        [Test]
        public void Constructor_FileHashOperationExceptionShouldBeThrown()
        {
            var testFile = TestSet1.First();
            HashUtility = Substitute.For<IHashUtility>();
            HashUtility.HashName.Returns("TEST HASH");
            HashUtility.HashLength.Returns(40);
            var exMessage = "Kablamo";
            HashUtility.GetHashFromFile("").ThrowsForAnyArgs(new Exception(exMessage));

            var exception = Assert.Throws<Exception>(() => new FileHasher(HashUtility, TestSourceFolder, testFile.FilePath));
            Assert.AreEqual(exMessage, exception.Message, "Expected exception message to be the same as that thrown by the hashing utility.");
        }

        [Test]
        public void ToString_ShouldReturnHashAndPath()
        {
            var testFile = TestSet1.First();
            CreateFile(testFile);
            var hasher = new FileHasher(HashUtility, TestSourceFolder, testFile.FilePath);
            var toString = hasher.ToString();

            Assert.AreEqual($"{testFile.Hash} {testFile.RelativePath}", toString, "Unexpected ToString value.");
        }

    }
}
