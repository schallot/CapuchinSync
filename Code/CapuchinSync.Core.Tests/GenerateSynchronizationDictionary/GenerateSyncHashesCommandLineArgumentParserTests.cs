using System;
using System.Linq;
using CapuchinSync.Core.GenerateSynchronizationDictionary;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.GenerateSynchronizationDictionary
{
    [TestFixture]
    public class GenerateSyncHashesCommandLineArgumentParserTests
    {
        private IFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.DoesDirectoryExist(Arg.Any<string>()).Returns(true);
        }

        [Test]
        public void Constructor_EverythingShouldBePeachy()
        {
            const string rootDir = "dir:blah";
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] {rootDir}, _fileSystem);

            Assert.AreEqual(Constants.EverythingsJustPeachyReturnCode, parser.ErrorNumber, "Expected everything to be peachy");
        }

        [Test]
        public void Constructor_RootDirectoryShouldBeParsedFromFirstArgument()
        {
            const string rootDir = "dir:blah";
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { rootDir }, _fileSystem);

            Assert.IsNotNull(parser.Arguments, "Expected arguments to be non-null");
            Assert.AreEqual(1, parser.Arguments.RootDirectories.Length, "Expected root directories to contain a single entry.");
            Assert.AreEqual("blah", parser.Arguments.RootDirectories.First(), "Unexpected root directory");
        }

        [Test]
        public void Constructor_ExtensionsToExclude_ShouldBeEmptyWhenNoExtensionsAreSpecified()
        {
            const string rootDir = "dir:blah";
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { rootDir }, _fileSystem);

            Assert.IsNotNull(parser.Arguments, "Expected arguments to be non-null");
            Assert.IsTrue(!parser.Arguments.ExtensionsToExclude.Any(), "Expected an empty collection of extensions to exclude");
        }

        [Test]
        public void Constructor_ExtensionsToExclude_ShouldContainExtensionsSpecifiedAtCommandline()
        {
            const string extension1 = "pdb";
            const string extension2 = "suo";
            const string rootDir = "dir:blah";
            const string pref = GenerateSyncHashesCommandLineArgumentParser.ExcludeFilePrefix;
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { rootDir, $"{pref}{extension1}", $"{pref}{extension2}" }, _fileSystem);

            Assert.IsNotNull(parser.Arguments, "Expected arguments to be non-null");
            Assert.IsNotNull(parser.Arguments.ExtensionsToExclude, $"Expected {nameof(parser.Arguments.ExtensionsToExclude)} to be non-null");
            var extensions = parser.Arguments.ExtensionsToExclude;
            Assert.AreEqual(2, extensions.Length, "Expected exactly two excluded extensions.");
            var first = extensions.FirstOrDefault(x=>x.Equals(extension1, StringComparison.InvariantCultureIgnoreCase));
            var second = extensions.FirstOrDefault(x=>x.Equals(extension2, StringComparison.InvariantCultureIgnoreCase));

            Assert.IsNotNull(first,$"Expected list of excluded extensions to include first extension {extension1}.  Instead, was [{string.Join(", ", extensions)}]");
            Assert.IsNotNull(second, $"Expected list of excluded extensions to include second {extension2}.  Instead, was [{string.Join(", ", extensions)}]");
        }

        [Test]
        public void Constructor_ExtensionsToExclude_ShouldNotCareAboutCaseOfPrefix()
        {
            const string extension1 = "pdb";
            const string extension2 = "suo";
            const string rootDir = "dir:blah";
            const string pref = GenerateSyncHashesCommandLineArgumentParser.ExcludeFilePrefix;
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { rootDir, $"{pref.ToUpperInvariant()}{extension1}", $"{pref.ToLowerInvariant()}{extension2}" }, _fileSystem);

            Assert.IsNotNull(parser.Arguments, "Expected arguments to be non-null");
            Assert.IsNotNull(parser.Arguments.ExtensionsToExclude, $"Expected {nameof(parser.Arguments.ExtensionsToExclude)} to be non-null");
            var extensions = parser.Arguments.ExtensionsToExclude;
            Assert.AreEqual(2, extensions.Length, "Expected exactly two excluded extensions.");
            var first = extensions.FirstOrDefault(x => x.Equals(extension1, StringComparison.InvariantCultureIgnoreCase));
            var second = extensions.FirstOrDefault(x => x.Equals(extension2, StringComparison.InvariantCultureIgnoreCase));

            Assert.IsNotNull(first, $"Expected list of excluded extensions to include first extension {extension1}.  Instead, was [{string.Join(", ", extensions)}]");
            Assert.IsNotNull(second, $"Expected list of excluded extensions to include second {extension2}.  Instead, was [{string.Join(", ", extensions)}]");
        }

        [Test]
        public void Constructor_ExtensionsToExclude_ShouldStripOffPeriods()
        {
            const string extension1 = "pdb";
            const string extension2 = "suo";
            const string rootDir = "dir:blah";
            const string pref = GenerateSyncHashesCommandLineArgumentParser.ExcludeFilePrefix;
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { rootDir, $"{pref.ToUpperInvariant()}.{extension1}", $"{pref.ToLowerInvariant()}.{extension2}" }, _fileSystem);

            Assert.IsNotNull(parser.Arguments, "Expected arguments to be non-null");
            Assert.IsNotNull(parser.Arguments.ExtensionsToExclude, $"Expected {nameof(parser.Arguments.ExtensionsToExclude)} to be non-null");
            var extensions = parser.Arguments.ExtensionsToExclude;
            Assert.AreEqual(2, extensions.Length, "Expected exactly two excluded extensions.");
            var first = extensions.FirstOrDefault(x => x.Equals(extension1, StringComparison.InvariantCultureIgnoreCase));
            var second = extensions.FirstOrDefault(x => x.Equals(extension2, StringComparison.InvariantCultureIgnoreCase));

            Assert.IsNotNull(first, $"Expected list of excluded extensions to include first extension {extension1}.  Instead, was [{string.Join(", ", extensions)}]");
            Assert.IsNotNull(second, $"Expected list of excluded extensions to include second {extension2}.  Instead, was [{string.Join(", ", extensions)}]");
        }

        [Test]
        public void Constructor_ExtensionsToExclude_ShouldStripOffStars()
        {
            const string extension1 = "pdb";
            const string extension2 = "suo";
            const string rootDir = "dir:blah";
            const string pref = GenerateSyncHashesCommandLineArgumentParser.ExcludeFilePrefix;
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { rootDir, $"{pref.ToUpperInvariant()}*{extension1}", $"{pref.ToLowerInvariant()}*{extension2}" }, _fileSystem);

            Assert.IsNotNull(parser.Arguments, "Expected arguments to be non-null");
            Assert.IsNotNull(parser.Arguments.ExtensionsToExclude, $"Expected {nameof(parser.Arguments.ExtensionsToExclude)} to be non-null");
            var extensions = parser.Arguments.ExtensionsToExclude;
            Assert.AreEqual(2, extensions.Length, "Expected exactly two excluded extensions.");
            var first = extensions.FirstOrDefault(x => x.Equals(extension1, StringComparison.InvariantCultureIgnoreCase));
            var second = extensions.FirstOrDefault(x => x.Equals(extension2, StringComparison.InvariantCultureIgnoreCase));

            Assert.IsNotNull(first, $"Expected list of excluded extensions to include first extension {extension1}.  Instead, was [{string.Join(", ", extensions)}]");
            Assert.IsNotNull(second, $"Expected list of excluded extensions to include second {extension2}.  Instead, was [{string.Join(", ", extensions)}]");
        }

        [Test]
        public void Constructor_ShouldTriggerNoArgumentsProvidedErrorCode()
        {
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new string[] { }, _fileSystem);
            Assert.AreEqual(GenerateSyncHashesCommandLineArgumentParser.ErrorCodes.NoArgumentsProvided, parser.ErrorNumber);
        }

        [Test]
        public void Constructor_ShouldTriggerDirectoryDoesNotExistErrorCode()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.DoesDirectoryExist(Arg.Any<string>()).Returns(false);
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new [] { "dir:thisIsTheDirectoryArgumentAndShouldBeFine" }, _fileSystem);
            Assert.AreEqual(GenerateSyncHashesCommandLineArgumentParser.ErrorCodes.DirectoryDoesNotExist, parser.ErrorNumber);
        }

        [Test]
        public void Constructor_ShouldTriggerUnrecognizedArgumentErrorCode()
        {
            const string directoryArg = "dir:thisIsTheDirectoryArgumentAndShouldBeFine";
            const string invalidPrefixArg = "invalidPrefix:whatever";
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.DoesDirectoryExist(Arg.Any<string>()).Returns(x =>
            {
                var arg = x[0] as string;
                if (arg == invalidPrefixArg) return false;
                return true;
            });
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new [] { directoryArg, invalidPrefixArg}, _fileSystem);
            Assert.AreEqual(GenerateSyncHashesCommandLineArgumentParser.ErrorCodes.UnrecognizedArgument, parser.ErrorNumber);
        }

    }
}