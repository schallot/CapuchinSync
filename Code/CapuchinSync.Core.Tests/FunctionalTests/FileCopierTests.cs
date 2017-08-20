using System;
using System.IO;
using System.Linq;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.FunctionalTests
{
    public class FileCopierTests : FileTestBase
    {
        [Test]
        public void PerformCopy_ShouldCopyFileToNewFile()
        {
            var source = TestSet1.First();
            CreateFile(source);
            var dest = Path.Combine(TestSourceFolder, Guid.NewGuid().ToString("N") + "_TestCopy\\testcopy.txt");

            var copier = new FileCopier(FileSystem, PathUtility, source.FilePath, dest);
            copier.PerformCopy();
            Assert.IsTrue(copier.SuccesfullyCopied, "Expected a successful copy.");
            Assert.IsTrue(File.Exists(dest), $"Expected file {dest} to exist");
            var contents = File.ReadAllText(dest);
            Assert.AreEqual(source.Contents, contents, $"Unexpected contents in file {dest}");
        }

        [Test]
        public void PerformCopy_ShouldCopyFileOverExistingFile()
        {
            var source = TestSet1.First();
            CreateFile(source);
            var dest = Path.Combine(TestSourceFolder, Guid.NewGuid().ToString("N") + "_testcopy.txt");
            
            File.WriteAllText(dest, "This text should be overwritten if FileCopier is working correctly.");
            
            var copier = new FileCopier(FileSystem, PathUtility, source.FilePath, dest);
            copier.PerformCopy();
            Assert.IsTrue(copier.SuccesfullyCopied, "Expected a successful copy.");
            Assert.IsTrue(File.Exists(dest), $"Expected file {dest} to exist");
            var contents = File.ReadAllText(dest);
            Assert.AreEqual(source.Contents, contents, $"Unexpected contents in file {dest}");
        }

        [Test]
        public void PerformCopy_ShouldFailIfDestinationDirectoryCannotBeDetermined()
        {
            var source = TestSet1.First();
            CreateFile(source);
            var dest = Path.Combine(TestSourceFolder, Guid.NewGuid().ToString("N") + "_testcopy.txt");

            File.WriteAllText(dest, "This text should be overwritten if FileCopier is working correctly.");

            PathUtility = Substitute.For<IPathUtility>();
            PathUtility.GetParentDirectoryFromPath("").ThrowsForAnyArgs(new Exception("This is gonna fail."));

            var copier = new FileCopier(FileSystem, PathUtility, source.FilePath, dest);

            copier.PerformCopy();
            Assert.IsFalse(copier.SuccesfullyCopied, "Expected a failed copy.");
        }

        [Test]
        public void PerformCopy_ShouldFailIfDestinationIsCalculatedToBeBlankString()
        {
            var source = TestSet1.First();
            CreateFile(source);
            var dest = Path.Combine(TestSourceFolder, Guid.NewGuid().ToString("N") + "_testcopy.txt");

            File.WriteAllText(dest, "This text should be overwritten if FileCopier is working correctly.");

            PathUtility = Substitute.For<IPathUtility>();
            PathUtility.GetParentDirectoryFromPath("").ReturnsForAnyArgs("");

            var copier = new FileCopier(FileSystem, PathUtility, source.FilePath, dest);

            copier.PerformCopy();
            Assert.IsFalse(copier.SuccesfullyCopied, "Expected a failed copy.");
        }

        [Test]
        public void PerformCopy_ShouldFailIfTargetDirectoryCannotBeCreated()
        {
            var source = TestSet1.First();
            CreateFile(source);
            var dest = Path.Combine(TestSourceFolder, Guid.NewGuid().ToString("N") + "_testcopy.txt");

            File.WriteAllText(dest, "This text should be overwritten if FileCopier is working correctly.");

            FileSystem = Substitute.For<IFileSystem>();
            FileSystem.DoesDirectoryExist("").ReturnsForAnyArgs(false);
            FileSystem.CreateDirectory("").ThrowsForAnyArgs(new Exception("This is gonna fail."));

            var copier = new FileCopier(FileSystem, PathUtility, source.FilePath, dest);

            copier.PerformCopy();
            Assert.IsFalse(copier.SuccesfullyCopied, "Expected a failed copy.");
        }

        [Test]
        public void PerformCopy_ShouldFailIfFileCopyFails()
        {
            var source = TestSet1.First();
            CreateFile(source);
            var dest = Path.Combine(TestSourceFolder, Guid.NewGuid().ToString("N") + "_testcopy.txt");

            File.WriteAllText(dest, "This text should be overwritten if FileCopier is working correctly.");

            FileSystem = Substitute.For<IFileSystem>();
            FileSystem.DoesDirectoryExist("").ReturnsForAnyArgs(true);
            FileSystem.CreateDirectory("").ReturnsForAnyArgs(true);
            FileSystem.WhenForAnyArgs(x=>x.CopyFileAndOverwriteIfItExists("",""))
                .Throw(new Exception("copy failed."));

            var copier = new FileCopier(FileSystem, PathUtility, source.FilePath, dest);

            copier.PerformCopy();
            Assert.IsFalse(copier.SuccesfullyCopied, "Expected a failed copy.");
        }

        [Test]
        public void Constructor_ShouldThrowNullArgumentExceptionForFileSystemArgument()
        {
            Assert.Throws<ArgumentNullException>( () =>
            new FileCopier(null, PathUtility, TestSourceFolder + "\\1.txt", TestDestinationFolder + "\\1.txt"));
        }

        [Test]
        public void Constructor_ShouldThrowNullArgumentExceptionForPathUtilityArgument()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FileCopier(FileSystem, null, TestSourceFolder + "\\1.txt", TestDestinationFolder + "\\1.txt"));
        }
    }
}
