using NUnit.Framework;
using CapuchinSync.Core.Interfaces;
using NSubstitute;

namespace CapuchinSync.Core.Tests
{
    [TestFixture]
    public class FileHasherFactoryTests
    {
        private IHashUtility _hash;
        private IPathUtility _pathUtility;
        [SetUp]
        public void SetUp()
        {
            _hash = Substitute.For<IHashUtility>();
            _hash.HashLength.Returns(5);
            _hash.HashName.Returns("TestHash");
            _hash.GetHashFromFile(Arg.Any<string>()).Returns("abcd0");
            _pathUtility = Substitute.For<IPathUtility>();
        }

        [Test]
        public void Constructor_ShouldCreateWithoutThrowingException()
        {
            var factory = new FileHasherFactory(_hash, _pathUtility);
        }

        [Test]
        public void CreateHasherTest()
        {
            var factory = new FileHasherFactory(_hash, _pathUtility);
            var hasher = factory.CreateHasher("C:\\temp\\blah.txt");
            Assert.IsNotNull(hasher, "Null hasher was returned");
            Assert.AreEqual(typeof(FileHasher),hasher.GetType(), "Unexpected type of hasher returned.");
        }
    }
}