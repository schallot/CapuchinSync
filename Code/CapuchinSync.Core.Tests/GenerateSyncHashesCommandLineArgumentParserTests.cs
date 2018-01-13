using CapuchinSync.Core.GenerateSynchronizationDictionary;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace CapuchinSync.Core.Tests
{
    [TestFixture]
    public class GenerateSyncHashesCommandLineArgumentParserTests
    {

        [Test]
        public void Constructor_ExistingDirectoryShouldResultInSuccessfulParse()
        {
            var directory = "aDirectory";
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.DoesDirectoryExist("").ReturnsForAnyArgs(true);

            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { $"{GenerateSyncHashesCommandLineArgumentParser.DirectoryPrefix}{directory}" }, fileSystem);

            Assert.AreEqual(parser.ErrorNumber, 0);
            var args = parser.Arguments;
            Assert.IsNotNull(args, "Null arguments were found");
            var extensionsToExclude = args.ExtensionsToExclude;
            Assert.IsNotNull(extensionsToExclude, "Extensions to exclude was expected to be an empty collection, but was null.");
            Assert.IsTrue(!extensionsToExclude.Any(), $"Expected an empty collection of extensions to exclude, found [{string.Join(", ", extensionsToExclude)}]");
            var directories = args.RootDirectories;
            Assert.IsNotNull(directories, "RootDirectories was unexpectedly null.");
            Assert.AreEqual(1, directories.Length, $"Expected a single directory, but found {directories.Length}: [{string.Join(", ", directories)}]");
            Assert.AreEqual(directory, directories.Single(), "Unexpected directory.");
        }

        [Test]
        public void Constructor_ShouldThrowErrorOnMissingDirectoryByDefault()
        {
            var missingDir = "missingDir";
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.DoesDirectoryExist("").ReturnsForAnyArgs(false);

            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] {$"{GenerateSyncHashesCommandLineArgumentParser.DirectoryPrefix}{missingDir}"}, fileSystem);
            Assert.AreEqual(parser.ErrorNumber, GenerateSyncHashesCommandLineArgumentParser.ErrorCodes.DirectoryDoesNotExist);
        }

        [Test]
        public void Constructor_MissingDirectoryShouldResultInSuccessfulParseWithIgnoreMissingDirectoriesArgument()
        {
            var existingDirectory = "anExistingDirectory";
            var missingDirectory = "aMissingDirectory";
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.DoesDirectoryExist(existingDirectory).Returns(true);
            fileSystem.DoesDirectoryExist(missingDirectory).Returns(false);

            var parser = new GenerateSyncHashesCommandLineArgumentParser(
                new[]
                {
                    $"{GenerateSyncHashesCommandLineArgumentParser.DirectoryPrefix}{missingDirectory}",
                    $"{GenerateSyncHashesCommandLineArgumentParser.DirectoryPrefix}{existingDirectory}",
                    GenerateSyncHashesCommandLineArgumentParser.IgnoreMissingDirectoriesArg
                }, fileSystem);

            Assert.AreEqual(parser.ErrorNumber, 0);
            var args = parser.Arguments;
            Assert.IsNotNull(args, "Null arguments were found");
            var extensionsToExclude = args.ExtensionsToExclude;
            Assert.IsNotNull(extensionsToExclude, "Extensions to exclude was expected to be an empty collection, but was null.");
            Assert.IsTrue(!extensionsToExclude.Any(), $"Expected an empty collection of extensions to exclude, found [{string.Join(", ", extensionsToExclude)}]");
            var directories = args.RootDirectories;
            Assert.IsNotNull(directories, "RootDirectories was unexpectedly null.");
            Assert.AreEqual(1, directories.Length, $"Expected a single directory, but found {directories.Length}: [{string.Join(", ", directories)}]");
            Assert.AreEqual(existingDirectory, directories.Single(), "Unexpected directory.");
        }
    }
}
