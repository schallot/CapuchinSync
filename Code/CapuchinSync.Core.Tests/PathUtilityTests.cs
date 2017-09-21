using System.IO;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests
{
    [TestFixture]
    public class PathUtilityTests
    {
        private PathUtility _pathUtility;
        [SetUp]
        public void SetUp()
        {
            _pathUtility = new PathUtility();
        }

        [Test]
        public void GetParentDirectoryFromPathTest()
        {
            Assert.AreEqual("C:\\Temp", _pathUtility.GetParentDirectoryFromPath("C:\\Temp\\Blah"));
        }

        [Test]
        public void CombineTest()
        {
            Assert.AreEqual("C:\\Temp\\Blah", _pathUtility.Combine("C:\\Temp\\","Blah"));
        }

        [Test]
        public void GetTempFileNameTest()
        {
            var file = _pathUtility.GetTempFileName();
            Assert.IsTrue(File.Exists(file),$"Expected temp file {file} to have already been created");
            var info = new FileInfo(file);
            Assert.AreEqual(0,info.Length, "Expected temp file to be zero bytes in length.");
            File.Delete(file);
        }

        [Test]
        public void GetFileNameTest()
        {
            Assert.AreEqual("blah",_pathUtility.GetFileName("C:\\Temp\\blah"));
        }

        [TestCase(@"c:\foo", @"c:", ExpectedResult = true)]
        [TestCase(@"c:\foo", @"c:\", ExpectedResult = true)]
        [TestCase(@"c:\foo", @"c:\foo", ExpectedResult = true)]
        [TestCase(@"c:\foo", @"c:\foo\", ExpectedResult = true)]
        [TestCase(@"c:\foo\", @"c:\foo", ExpectedResult = true)]
        [TestCase(@"c:\foo\bar\", @"c:\foo\", ExpectedResult = true)]
        [TestCase(@"c:\foo\bar", @"c:\foo\", ExpectedResult = true)]
        [TestCase(@"c:\foo\a.txt", @"c:\foo", ExpectedResult = true)]
        [TestCase(@"c:\FOO\a.txt", @"c:\foo", ExpectedResult = true)]
        [TestCase(@"c:/foo/a.txt", @"c:\foo", ExpectedResult = true)]
        [TestCase(@"c:\foobar", @"c:\foo", ExpectedResult = false)]
        [TestCase(@"c:\foobar\a.txt", @"c:\foo", ExpectedResult = false)]
        [TestCase(@"c:\foobar\a.txt", @"c:\foo\", ExpectedResult = false)]
        [TestCase(@"c:\foo\a.txt", @"c:\foobar", ExpectedResult = false)]
        [TestCase(@"c:\foo\a.txt", @"c:\foobar\", ExpectedResult = false)]
        [TestCase(@"c:\foo\..\bar\baz", @"c:\foo", ExpectedResult = false)]
        [TestCase(@"c:\foo\..\bar\baz", @"c:\bar", ExpectedResult = true)]
        [TestCase(@"c:\foo\..\bar\baz", @"c:\barr", ExpectedResult = false)]
        public bool IsSubPathOfTest(string path, string baseDirPath)
        {
            return _pathUtility.IsSubPathOrEqualTo(baseDirPath, path);
        }
    }
}