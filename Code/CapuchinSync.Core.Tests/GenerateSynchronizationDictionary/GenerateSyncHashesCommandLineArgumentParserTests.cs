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
            const string rootDir = "blah";
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] {rootDir}, _fileSystem);

            Assert.AreEqual(Constants.EverythingsJustPeachyReturnCode, parser.ErrorNumber, "Expected everything to be peachy");
        }

        [Test]
        public void Constructor_RootDirectoryShouldBeParsedFromFirstArgument()
        {
            const string rootDir = "blah";
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { rootDir }, _fileSystem);

            Assert.IsNotNull(parser.Arguments, "Expected arguments to be non-null");
            Assert.AreEqual(rootDir, parser.Arguments.RootDirectory, "Unexpected root directory");
        }

        [Test]
        public void Constructor_ExtensionsToExclude_ShouldBeEmptyWhenNoExtensionsAreSpecified()
        {
            const string rootDir = "blah";
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new[] { rootDir }, _fileSystem);

            Assert.IsNotNull(parser.Arguments, "Expected arguments to be non-null");
            Assert.IsTrue(!parser.Arguments.ExtensionsToExclude.Any(), "Expected an empty collection of extensions to exclude");
        }

        [Test]
        public void Constructor_ExtensionsToExclude_ShouldContainExtensionsSpecifiedAtCommandline()
        {
            const string extension1 = "pdb";
            const string extension2 = "suo";
            const string rootDir = "blah";
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
            const string rootDir = "blah";
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
            const string rootDir = "blah";
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
            const string rootDir = "blah";
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
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new [] { "thisIsTheDirectoryArgumentAndShouldBeFine" }, _fileSystem);
            Assert.AreEqual(GenerateSyncHashesCommandLineArgumentParser.ErrorCodes.DirectoryDoesNotExist, parser.ErrorNumber);
        }

        [Test]
        public void Constructor_ShouldTriggerUnrecognizedArgumentErrorCode()
        {
            var parser = new GenerateSyncHashesCommandLineArgumentParser(new [] { "thisIsTheDirectoryArgumentAndShouldBeFine", "invalidPrefix:whatever"}, _fileSystem);
            Assert.AreEqual(GenerateSyncHashesCommandLineArgumentParser.ErrorCodes.UnrecognizedArgument, parser.ErrorNumber);
        }

    }
}