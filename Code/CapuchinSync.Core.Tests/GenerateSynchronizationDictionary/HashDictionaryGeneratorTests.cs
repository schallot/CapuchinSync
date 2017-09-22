using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CapuchinSync.Core.GenerateSynchronizationDictionary;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.GenerateSynchronizationDictionary
{
    [TestFixture]
    public class HashDictionaryGeneratorTests
    {
        private HashDictionaryGenerator _generator;
        private GenerateSyncHashesArguments _arguments;
        private IFileSystem _fileSystem;
        private IPathUtility _pathUtility;
        private IFileHasherFactory _fileHasherFactory;
        private IDateTimeProvider _dateTimeProvider;

        private const string TestRootDir = "ThisIsATestRootDirectory";
        private const string File1 = "File1";
        private const string File2 = "File2";
        private List<Tuple<string, string>> _filesWritten;
        private const string NowTime = "Beer o'clock";
        private const string NowDay = "Blue Monday";
        private const string DefaultHash = "SomeHash";
        private const string File1DictionaryEntry = DefaultHash + " " + File1;
        private const string File2DictionaryEntry = DefaultHash + " " + File2;
        private List<string> Files;

        [SetUp]
        public void SetUp()
        {
            _filesWritten = new List<Tuple<string, string>>();
            Files = new List<string>{File1, File2};
            _arguments = new GenerateSyncHashesArguments {RootDirectories = new []{TestRootDir}};
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.EnumerateFilesInDirectory(TestRootDir).Returns(Files);
            _fileSystem.When(x=>x.WriteAsUtf8TextFile(Arg.Any<string>(), Arg.Any<string>())).Do(y =>
            {
                _filesWritten.Add(new Tuple<string, string>(y[0] as string, y[1] as string));
            });

            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _dateTimeProvider.GetDateString(Arg.Any<DateTime>()).Returns(NowDay);
            _dateTimeProvider.GetTimeString(Arg.Any<DateTime>()).Returns(NowTime);
            _dateTimeProvider.Now.Returns(DateTime.Now);
            
            _pathUtility = Substitute.For<IPathUtility>();
            _pathUtility.Combine(Arg.Any<string>(), Arg.Any<string>()).Returns(x => $"{x[0] as string}\\{x[1] as string}");
            _pathUtility.GetFileName(Arg.Any<string>()).Returns(x => Path.GetFileName(x[0] as string));

            var firstHasher = Substitute.For<IFileHasher>();
            firstHasher.GetDictionaryEntryString(Arg.Any<string>()).Returns(File1DictionaryEntry);
            firstHasher.FullPath.Returns(File1);
            var secondHasher = Substitute.For<IFileHasher>();
            secondHasher.GetDictionaryEntryString(Arg.Any<string>()).Returns(File2DictionaryEntry);
            firstHasher.FullPath.Returns(File2);

            _fileHasherFactory = Substitute.For<IFileHasherFactory>();
            _fileHasherFactory.CreateHasher(File1).Returns(firstHasher);
            _fileHasherFactory.CreateHasher(File2).Returns(secondHasher);
        }

        [Test]
        public void Constructor_BasicHashTest()
        {
            _pathUtility.IsSubPathOrEqualTo(Arg.Any<string>(), Arg.Any<string>()).Returns(x =>
            {
                var arg1 = x[0] as string;
                if (arg1 == TestRootDir) return true;
                return false;
            }); // We only have one root directory to worry about here, so all paths we work with will be in that directory.
            

            _generator = new HashDictionaryGenerator(_arguments, _fileSystem, _pathUtility, _fileHasherFactory, _dateTimeProvider);
            Assert.AreEqual(1, _filesWritten.Count, "Expected exactly one file to have been written.");
            Assert.AreEqual($"{TestRootDir}\\{Constants.HashFileName}",_filesWritten.First().Item1, "Written to unexpected file");
            var contents = _filesWritten.First().Item2;
            Assert.IsNotNull(contents, "No file contents were found.");
            var lines = contents.Split('\r', '\n').Where(x=>!string.IsNullOrWhiteSpace(x)).Select(x=>x.Trim()).ToList();
            Assert.AreEqual(3,lines.Count, "Expected exactly three lines in the dictionary file.");
            Assert.AreEqual($"2 files found in {TestRootDir} on {NowDay} at {NowTime}", lines[0], "Unexpected header line.");
            lines.RemoveAt(0);
            Assert.IsTrue(lines.Any(x => x == File1DictionaryEntry), $"No lines matched <{File1DictionaryEntry}> in {contents}");
            Assert.IsTrue(lines.Any(x => x == File2DictionaryEntry), $"No lines matched <{File2DictionaryEntry}> in {contents}");
        }

        [Test]
        public void Constructor_NullArgumentsShouldThrowNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _generator = new HashDictionaryGenerator(null, _fileSystem, _pathUtility, _fileHasherFactory, _dateTimeProvider);
            }, $"Expected a null argument exception to be thrown for null {nameof(GenerateSyncHashesArguments)} argument.");
        }

        private static IEnumerable<string> ErrorOnThirdItem()
        {
            for (int i = 0; i < 4 ; i++)
            {
                if (i == 0)
                    yield return "FirstItem";
                if (i == 1)
                    yield return "SecondItem";
                if (i == 2)
                {
                    throw new Exception("BLAH!");
                }
                yield return "FourthItem";
            }
        }

        [Test]
        public void Constructor_ShouldThrowEnumerationException()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.EnumerateFilesInDirectory(Arg.Any<string>()).Returns(ErrorOnThirdItem());
            var ex = Assert.Throws<Exception>(() =>
            {
                _generator = new HashDictionaryGenerator(_arguments, _fileSystem, _pathUtility, _fileHasherFactory, _dateTimeProvider);
            });
            
            Assert.IsTrue(ex.Message.Contains($"Enumeration of a directory failed: BLAH!"));
        }
        
        [Test]
        public void Constructor_ShouldCopyOldFileToBackupLocationAndOverwriteOldBackupFile()
        {
            const string dictionaryFile = "dictionaryFile";
            const string backupFile = dictionaryFile + ".old";
            _pathUtility = Substitute.For<IPathUtility>();
            _pathUtility.Combine(Arg.Any<string>(), Arg.Any<string>()).Returns(dictionaryFile);

            bool wasFileMoved = false;
            bool wasFileWritten = false;
            bool wasOldBackupFileDeleted = false;

            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.EnumerateFilesInDirectory(Arg.Any<string>()).Returns(new [] {"one", "two"});
            _fileSystem.DoesFileExist(dictionaryFile).Returns(true);
            _fileSystem.DoesFileExist(backupFile).Returns(true);
            _fileSystem.When(x=>x.DeleteFile(backupFile)).Do(x=> { wasOldBackupFileDeleted = true; });
            _fileSystem.When(x=>x.MoveFile(dictionaryFile, backupFile)).Do(x =>
            {
                wasFileMoved = true;
            });
            _fileSystem.When(x=>x.WriteAsUtf8TextFile(dictionaryFile,Arg.Any<string>())).Do(x =>
            {
                wasFileWritten = true;
            });

            _generator = new HashDictionaryGenerator(_arguments, _fileSystem, _pathUtility, _fileHasherFactory, _dateTimeProvider);

            Assert.IsTrue(wasFileMoved, "The old dictionary file does not appear to have been moved to the backup location.");
            Assert.IsTrue(wasFileWritten, "The new dictionary file does not appear to have been written.");
            Assert.IsTrue(wasOldBackupFileDeleted, "The old backup file does not appear to have been deleted.");
        }

        [Test]
        public void Constructor_ShouldExcludeExistingHashDictionaryFile()
        {
            Files.Add(Constants.HashFileName);
            _generator = new HashDictionaryGenerator(_arguments, _fileSystem, _pathUtility, _fileHasherFactory, _dateTimeProvider);
            Assert.AreEqual(1, _filesWritten.Count, "Expected exactly one file to have been written.");
            Assert.AreEqual($"{TestRootDir}\\{Constants.HashFileName}", _filesWritten.First().Item1, "Written to unexpected file");
            var contents = _filesWritten.First().Item2;
            Assert.IsNotNull(contents, "No file contents were found.");
            Assert.IsFalse(contents.ToUpperInvariant().Contains(Constants.HashFileName.ToUpperInvariant()), "Expected the hash dictionary file to be excluded from the newest hash dictionary.");
        }

    }
}