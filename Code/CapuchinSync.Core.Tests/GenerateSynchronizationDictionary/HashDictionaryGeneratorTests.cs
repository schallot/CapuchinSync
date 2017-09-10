using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private List<Tuple<string, string>> _filesWritten = new List<Tuple<string, string>>();
        private const string NowTime = "Beer o'clock";
        private const string NowDay = "Blue Monday";
        private const string DefaultHash = "SomeHash";
        private const string File1DictionaryEntry = DefaultHash + " " + File1;
        private const string File2DictionaryEntry = DefaultHash + " " + File2;


        [SetUp]
        public void SetUp()
        {
            _arguments = new GenerateSyncHashesArguments {RootDirectory = TestRootDir};
            _fileSystem = Substitute.For<IFileSystem>();
            _fileSystem.EnumerateFilesInDirectory(TestRootDir).Returns(new[]{File1, File2});
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

            var firstHasher = Substitute.For<IFileHasher>();
            firstHasher.DictionaryEntryString.Returns(File1DictionaryEntry);
            var secondHasher = Substitute.For<IFileHasher>();
            secondHasher.DictionaryEntryString.Returns(File2DictionaryEntry);

            _fileHasherFactory = Substitute.For<IFileHasherFactory>();
            _fileHasherFactory.CreateHasher(File1).Returns(firstHasher);
            _fileHasherFactory.CreateHasher(File2).Returns(secondHasher);
        }

        [Test]
        public void BasicHashTest()
        {
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
        public void NullArguments_ShouldThrowNullArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _generator = new HashDictionaryGenerator(null, _fileSystem, _pathUtility, _fileHasherFactory, _dateTimeProvider);
            }, $"Expected a null argument exception to be thrown for null {nameof(GenerateSyncHashesArguments)} argument.");
        }


    }
}