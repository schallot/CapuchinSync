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
    }
}
