using System;
using System.IO;
using System.Linq;
using CapuchinSync.Core.DirectorySynchronization;
using CapuchinSync.Core.Hashes;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.FunctionalTests
{
    public class DirectorySyncherTests : FileTestBase
    {
        [Test]
        public void SynchronizeTest()
        {
            WriteTestSet(TestSet1);

            var sets = TestSet1.Select(x=> new HashVerifier(
                    new HashDictionaryEntry(HashUtility, TestSourceFolder, x.HashFileLine), TestDestinationFolder, 
                    FileSystem, PathUtility, HashUtility
                )).ToList();

            var syncher = new DirectorySyncher(FileSystem, PathUtility, FileCopierFactory)
            {
                OpenLogInNotepad = false
            };
            syncher.Synchronize(sets);

            var expectedFiles = TestSet1.Select(x => new
            {
                x.Contents,
                Path = Path.Combine(TestDestinationFolder, x.RelativePath)
            }).ToArray();

            foreach (var expectedFile in expectedFiles)
            {
                Assert.IsTrue(File.Exists(expectedFile.Path), $"Target file {expectedFile.Path} does not exist.");
                var actualContents = File.ReadAllText(expectedFile.Path);
                Assert.AreEqual(expectedFile.Contents, actualContents, $"Unexpected contents of target file {expectedFile.Path}.");
            }
        }

        [Test]
        public void ConstructorTest_ShouldThrowArgumentNullExceptionWithNullFileSystem()
        {
            Assert.Throws<ArgumentNullException>(() => new DirectorySyncher(null, PathUtility, FileCopierFactory));
        }

        [Test]
        public void ConstructorTest_ShouldThrowArgumentNullExceptionWithNullPathUtility()
        {
            Assert.Throws<ArgumentNullException>(() => new DirectorySyncher(FileSystem, null, FileCopierFactory));
        }

        [Test]
        public void ConstructorTest_ShouldThrowArgumentNullExceptionWithFileCopierFactory()
        {
            Assert.Throws<ArgumentNullException>(() => new DirectorySyncher(FileSystem, PathUtility, null));
        }
    }
}
