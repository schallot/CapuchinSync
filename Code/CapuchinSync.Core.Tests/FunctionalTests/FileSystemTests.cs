using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.FunctionalTests
{
    [TestFixture]
    public class FileSystemTests : FileTestBase
    {
        [Test]
        public void ShouldEnumerateFiles()
        {
            var testSet = TestSet1.ToList();
            WriteTestSet(testSet);

            var results = FileSystem.EnumerateFilesInDirectory(TestSourceFolder).ToList();

            var misMatches1 =
                results.Where(x => !testSet.Any(y => y.FilePath.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
                    .ToArray();
            var misMatches2 = testSet.Where(x => !results.Any(y => y.Equals(x.FilePath, StringComparison.InvariantCultureIgnoreCase)))
                    .ToArray();

            if (misMatches1.Any())
            {
                Assert.Fail($"Found {misMatches1.Length} items in the results that were not in the test set: [{string.Join(", ", misMatches1)}].");
            }
            if (misMatches2.Any())
            {
                Assert.Fail($"Found {misMatches2.Length} items in the test set that were not in the results: [{string.Join(", ", misMatches2.Select(y=>y.FilePath))}].");
            }
        }

        [Test]
        public void DoesFileExist_ShouldBeSuccessful()
        {
            var testFile = TestSet1.First();
            CreateFile(testFile);
            Assert.IsTrue(FileSystem.DoesFileExist(testFile.FilePath), "Expected test file to be found.");
        }

        [Test]
        public void DoesDirectoryExist_ShouldBeSuccessful()
        {
            var testFile = TestSet1.First();
            CreateFile(testFile);
            var dir = Path.GetDirectoryName(testFile.FilePath);
            Assert.IsTrue(FileSystem.DoesDirectoryExist(dir), "Expected test directory to be found.");
        }

        [Test]
        public void CreateDirectory_ShouldBeSuccessful()
        {
            var directory = Path.Combine(TestSourceFolder, Guid.NewGuid().ToString("N"));
            var result = FileSystem.CreateDirectory(directory);
            Assert.IsTrue(result, "Expected CreateDirectory to return true.");
            Assert.IsTrue(Directory.Exists(directory), $"Expected directory {directory} to have been created.");
        }

        [Test]
        public void CopyFileAndOverwriteIfItExists_ShouldBeSuccessfulWhenTargetFileDoesNotExist()
        {
            var fileGuid = Guid.NewGuid();
            var source = Path.Combine(TestSourceFolder, fileGuid.ToString("N") + "_source.txt");
            var contents = "Hello, I'm a file " + Guid.NewGuid().ToString("D");
            File.WriteAllText(source, contents);
            var target = Path.Combine(TestSourceFolder, fileGuid.ToString("N") + "_target.txt");
            FileSystem.CopyFileAndOverwriteIfItExists(source, target);
            Assert.IsTrue(File.Exists(target), $"Expected target file to exist at {target}.");
            var actualContents = File.ReadAllText(target);
            Assert.AreEqual(contents, actualContents, $"Unexpected contents at {target}.");
        }

        [Test]
        public void CopyFileAndOverwriteIfItExists_ShouldBeSuccessfulWhenTargetFileExists()
        {
            var fileGuid = Guid.NewGuid();
            var source = Path.Combine(TestSourceFolder, fileGuid.ToString("N") + "_source.txt");
            var contents = "Hello, I'm a file " + Guid.NewGuid().ToString("D");
            File.WriteAllText(source, contents);
            var target = Path.Combine(TestSourceFolder, fileGuid.ToString("N") + "_target.txt");
            File.WriteAllText(target, "blah, this should get overwritten.");
            FileSystem.CopyFileAndOverwriteIfItExists(source, target);
            Assert.IsTrue(File.Exists(target), $"Expected target file to exist at {target}.");
            var actualContents = File.ReadAllText(target);
            Assert.AreEqual(contents, actualContents, $"Unexpected contents at {target}.");
        }

        [Test]
        public void ReadAllLines_ShouldBeSuccessful()
        {
            var fileGuid = Guid.NewGuid();
            var source = Path.Combine(TestSourceFolder, fileGuid.ToString("N") + "_source.txt");
            var line1 = "Hello, I'm a file";
            var line2 = Guid.NewGuid().ToString("D"); 
            File.WriteAllLines(source, new []{line1, line2});
            var lines = FileSystem.ReadAllLines(source).ToArray();
            Assert.AreEqual(2,lines.Length, "Expected two lines in in the file");
            Assert.AreEqual(line1, lines.First(), "Unexpected first line");
            Assert.AreEqual(line2, lines.Last(), "Unexpected second line");
        }

        [Test]
        public void DeleteFile_ShouldBeSuccessful()
        {
            var path = Path.Combine(TestSourceFolder, Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllText(path, "Hello");
            FileSystem.DeleteFile(path);
            Assert.IsFalse(File.Exists(path), "File was not deleted.");
        }

        [Test]
        public void MoveFile_ShouldBeSuccessful()
        {
            var fileGuid = Guid.NewGuid();
            var source = Path.Combine(TestSourceFolder, fileGuid.ToString("N") + "_source.txt");
            var contents = "Hello, I'm a file " + Guid.NewGuid().ToString("D");
            File.WriteAllText(source, contents);
            var target = Path.Combine(TestSourceFolder, fileGuid.ToString("N") + "_target.txt");
            FileSystem.MoveFile(source, target);
            Assert.IsTrue(File.Exists(target), $"Expected target file to exist at {target}.");
            var actualContents = File.ReadAllText(target);
            Assert.AreEqual(contents, actualContents, $"Unexpected contents at {target}.");
            Assert.IsFalse(File.Exists(source), "File still exists at original location.");
        }

        [Test]
        public void WriteAsUtf8_ShouldBeSuccessful()
        {
            var fileGuid = Guid.NewGuid();
            var target = Path.Combine(TestSourceFolder, fileGuid.ToString("N") + "_source.txt");
            var contents = "Hello, I'm a file " + Guid.NewGuid().ToString("D");
            FileSystem.WriteAsUtf8TextFile(target, contents);
            var actualContents = File.ReadAllText(target);
            Assert.AreEqual(contents, actualContents, $"Unexpected contents at {target}.");
        }
    }
}
