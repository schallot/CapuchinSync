using System;
using System.Collections.Generic;
using CapuchinSync.Core.DirectorySynchronization;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.DirectorySynchronization
{
    [TestFixture]
    public class DirectorySyncherTests
    {
        private IFileSystem _fileSystem;
        private IPathUtility _pathUtility;
        private IFileCopierFactory _fileCopierFactory;

        private IHashVerifier _unknownVerifier;
        private IHashVerifier _matchedVerifier;
        private IHashVerifier _misMatchedVerifier1;
        private IHashVerifier _misMatchedVerifier2;
        private IHashVerifier _missingTargetVerifier;
        private List<IHashVerifier> _verifiers;

        private TestFileCopier _unknownVerifierCopier;
        private TestFileCopier _mismatchedVerifierCopier1;
        private TestFileCopier _mismatchedVerifierCopier2;
        private TestFileCopier _missingTargetVerifierCopier;

        private const string RootSourcePath = "C:\\source";
        private const string RootTargetPath = "D:\\target";

        public class TestFileCopier : IFileCopier
        {
            public string Source { get; private set; }
            public string Destination { get; private set; }

            public void SetCopyInfo(CallInfo callInfo)
            {
                Source = callInfo[0].ToString();
                Destination = callInfo[1].ToString();
            }

            public bool SuccesfullyCopied { get; set; } = true;
            public bool PerformCopyWasCalled { get; private set; }
            public void PerformCopy()
            {
                PerformCopyWasCalled = true;
            }
        }

        public class TestHashVerifier : IHashVerifier
        {
            public TestHashVerifier(string calculatedHash, string name, string rootSourceDirectory, string rootTargetDirectory, HashVerifier.VerificationStatus status, string errorMessage = "")
            {
                CalculatedHash = calculatedHash;
                FullSourcePath = $"{name}FullSourcePath";
                FullTargetPath = $"{name}FullTargetPath";
                _hashEntry = new TestHashDictionaryEntry(errorMessage, calculatedHash, string.IsNullOrWhiteSpace(errorMessage), name, rootSourceDirectory);
                RootSourceDirectory = rootSourceDirectory;
                RootTargetDirectory = rootTargetDirectory;
                Status = status;
            }

            public class TestHashDictionaryEntry : IHashDictionaryEntry
            {
                public TestHashDictionaryEntry(string errorMessage, string hash, bool isValid, string relativePath, string rootDirectory)
                {
                    ErrorMessage = errorMessage;
                    Hash = hash;
                    IsValid = isValid;
                    RelativePath = relativePath;
                    RootDirectory = rootDirectory;
                }

                public string ErrorMessage { get; }
                public string Hash { get; }
                public bool IsValid { get; }
                public string RelativePath { get; }
                public string RootDirectory { get; }
            }

            public string CalculatedHash { get; }
            public string FullSourcePath { get; }
            public string FullTargetPath { get; }

            private readonly IHashDictionaryEntry _hashEntry;

            public IHashDictionaryEntry GetHashEntry()
            {
                return _hashEntry;
            }

            public string RootSourceDirectory { get; }
            public string RootTargetDirectory { get; }
            public HashVerifier.VerificationStatus Status { get; }
        }

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _pathUtility = Substitute.For<IPathUtility>();
            _fileCopierFactory = Substitute.For<IFileCopierFactory>();

            _unknownVerifierCopier = new TestFileCopier();
            _mismatchedVerifierCopier1 = new TestFileCopier();
            _mismatchedVerifierCopier2 = new TestFileCopier();
            _missingTargetVerifierCopier = new TestFileCopier();

            _unknownVerifier = new TestHashVerifier("123", "unknownVerifier", RootSourcePath, RootTargetPath, 
                HashVerifier.VerificationStatus.TargetFileNotRead);
            _matchedVerifier = new TestHashVerifier("456", "matchedVerifier", RootSourcePath, RootTargetPath,
                HashVerifier.VerificationStatus.TargetFileMatchesHash);
            // These two files have matching hashes, but only one of them should end up copied from the source path: one should be copied from within the target folder
            _misMatchedVerifier1 = new TestHashVerifier("789", "misMatchedVerifier1", RootSourcePath, RootTargetPath,
                HashVerifier.VerificationStatus.TargetFileDoesNotMatchHash);
            _misMatchedVerifier2 = new TestHashVerifier("789", "misMatchedVerifier2", RootSourcePath, RootTargetPath,
                HashVerifier.VerificationStatus.TargetFileDoesNotMatchHash);
            _missingTargetVerifier = new TestHashVerifier("0AB", "missingTargetVerifier", RootSourcePath, RootTargetPath,
                HashVerifier.VerificationStatus.TargetFileDoesNotExist);

            _fileCopierFactory = Substitute.For<IFileCopierFactory>();
            _fileCopierFactory.CreateFileCopier(_unknownVerifier.FullSourcePath, _unknownVerifier.FullTargetPath)
                .Returns(_unknownVerifierCopier);
            _fileCopierFactory.CreateFileCopier(_missingTargetVerifier.FullSourcePath, _missingTargetVerifier.FullTargetPath)
                .Returns(_missingTargetVerifierCopier);
            _fileCopierFactory.CreateFileCopier(_missingTargetVerifier.FullSourcePath, _missingTargetVerifier.FullTargetPath)
                .Returns(_missingTargetVerifierCopier);

            // For each mismatched file, we can't be sure if it's going to be copied from the source folder, or from 
            // the files' local match in the target directory, so we'll have to account for all options.
            _fileCopierFactory.CreateFileCopier(_misMatchedVerifier1.FullSourcePath, _misMatchedVerifier1.FullTargetPath)
                .Returns(_mismatchedVerifierCopier1)
                .AndDoes(x => _mismatchedVerifierCopier1.SetCopyInfo(x));
            _fileCopierFactory.CreateFileCopier(_misMatchedVerifier2.FullSourcePath, _misMatchedVerifier2.FullTargetPath)
                .Returns(_mismatchedVerifierCopier2)
                .AndDoes(x => _mismatchedVerifierCopier2.SetCopyInfo(x));
            _fileCopierFactory.CreateFileCopier(_misMatchedVerifier2.FullTargetPath, _misMatchedVerifier1.FullTargetPath)
                .Returns(_mismatchedVerifierCopier1)
                .AndDoes(x => _mismatchedVerifierCopier1.SetCopyInfo(x));
            _fileCopierFactory.CreateFileCopier(_misMatchedVerifier1.FullSourcePath, _misMatchedVerifier2.FullTargetPath)
                .Returns(_mismatchedVerifierCopier2)
                .AndDoes(x => _mismatchedVerifierCopier2.SetCopyInfo(x));

            _verifiers = new List<IHashVerifier>{_unknownVerifier, _matchedVerifier, _misMatchedVerifier2, _misMatchedVerifier1, _missingTargetVerifier};
        }

        [Test]
        public void Synchronize_ExpectAllUnverifiedFilesToBeCopiedOver()
        {
            var synchronizer =
                new DirectorySyncher(_fileSystem, _pathUtility, _fileCopierFactory) {OpenLogInNotepad = false};
            synchronizer.Synchronize(_verifiers);
         
            Assert.IsTrue(_mismatchedVerifierCopier1.PerformCopyWasCalled, "Expected mismatched verifier 1 to result in a file copy");
            Assert.IsTrue(_mismatchedVerifierCopier2.PerformCopyWasCalled, "Expected mismatched verifier 1 to result in a file copy");
            Assert.IsTrue(_missingTargetVerifierCopier.PerformCopyWasCalled, "Expected missing target verifier to result in a file copy");
            Assert.IsTrue(_unknownVerifierCopier.PerformCopyWasCalled, "Expected inability to generate hash to result in a file copy");

            Assert.AreEqual("misMatchedVerifier1FullTargetPath", _mismatchedVerifierCopier1.Destination, "_mismatchedVerifierCopier1 copied to an unexpected destination");
            Assert.AreEqual("misMatchedVerifier2FullTargetPath", _mismatchedVerifierCopier2.Destination, "_mismatchedVerifierCopier2 copied to an unexpected destination");

            if (_mismatchedVerifierCopier1.Source == "misMatchedVerifier1FullSourcePath")
            {
                Assert.AreEqual("misMatchedVerifier1FullTargetPath", _mismatchedVerifierCopier2.Source,
                    $"Since {nameof(_mismatchedVerifierCopier1)} was copied from it's full source path, " +
                    $"{nameof(_mismatchedVerifierCopier2)} should have been copied from  {nameof(_mismatchedVerifierCopier2)}'s target location, " +
                    $"but instead it was copied from {_mismatchedVerifierCopier2.Source}");
            }
            else if (_mismatchedVerifierCopier2.Source == "misMatchedVerifier2FullSourcePath")
            {
                Assert.AreEqual("misMatchedVerifier2FullTargetPath", _mismatchedVerifierCopier1.Source,
                    $"Since {nameof(_mismatchedVerifierCopier2)} was copied from it's full source path, " +
                    $"{nameof(_mismatchedVerifierCopier1)} should have been copied from  {nameof(_mismatchedVerifierCopier1)}'s target location, " +
                    $"but instead it was copied from {_mismatchedVerifierCopier1.Source}");
            }
            else
            {
                Assert.Fail($"Neither {nameof(_mismatchedVerifierCopier1)} nor {nameof(_mismatchedVerifierCopier2)} were copied from their respective FullSourcePaths, but instead they were {_mismatchedVerifierCopier1.Source} and {_mismatchedVerifierCopier2.Source}, respectively.");
            }
        }

        // ReSharper disable ObjectCreationAsStatement
        [Test]
        public void ConstructorTest_ShouldThrowArgumentNullExceptionWithNullFileSystem()
        {
            Assert.Throws<ArgumentNullException>(() => new DirectorySyncher(null, _pathUtility, _fileCopierFactory));
        }

        [Test]
        public void ConstructorTest_ShouldThrowArgumentNullExceptionWithNullPathUtility()
        {
            Assert.Throws<ArgumentNullException>(() => new DirectorySyncher(_fileSystem, null, _fileCopierFactory));
        }

        [Test]
        public void ConstructorTest_ShouldThrowArgumentNullExceptionWithFileCopierFactory()
        {
            Assert.Throws<ArgumentNullException>(() => new DirectorySyncher(_fileSystem, _pathUtility, null));
        }
        // ReSharper restore ObjectCreationAsStatement

    }
}