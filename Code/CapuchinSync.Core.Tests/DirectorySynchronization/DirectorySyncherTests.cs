using System;
using System.Collections.Generic;
using CapuchinSync.Core.DirectorySynchronization;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
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
        private IHashVerifier _misMatchedVerifier;
        private IHashVerifier _missingTargetVerifier;
        private List<IHashVerifier> _verifiers;

        private TestFileCopier _unknownVerifierCopier;
        private TestFileCopier _mismatchedVerifierCopier;
        private TestFileCopier _missingTargetVerifierCopier;

        private const string RootSourcePath = "C:\\source";
        private const string RootTargetPath = "D:\\target";

        public class TestFileCopier : IFileCopier
        {
            public string Source { get; set; }
            public string Destination { get; set; }
            public bool SuccesfullyCopied { get; set; } = true;
            public bool PerformCopyWasCalled { get; private set; }
            public void PerformCopy()
            {
                PerformCopyWasCalled = true;
            }
        }

        //public class TestFileCopierFactory : IFileCopierFactory
        //{
        //    public IFileCopier CreateFileCopier(string source, string destination)
        //    {
        //        return new TestFileCopier
        //        {
        //            Destination = destination,
        //            Source = source
        //        };
        //    }
        //}

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
            _mismatchedVerifierCopier = new TestFileCopier();
            _missingTargetVerifierCopier = new TestFileCopier();

            _unknownVerifier = new TestHashVerifier("123", "unknownVerifier", RootSourcePath, RootTargetPath, 
                HashVerifier.VerificationStatus.TargetFileNotRead);
            _matchedVerifier = new TestHashVerifier("456", "matchedVerifier", RootSourcePath, RootTargetPath,
                HashVerifier.VerificationStatus.TargetFileMatchesHash);
            _misMatchedVerifier = new TestHashVerifier("789", "misMatchedVerifier", RootSourcePath, RootTargetPath,
                HashVerifier.VerificationStatus.TargetFileDoesNotMatchHash);
            _missingTargetVerifier = new TestHashVerifier("0AB", "missingTargetVerifier", RootSourcePath, RootTargetPath,
                HashVerifier.VerificationStatus.TargetFileDoesNotExist);

            _fileCopierFactory = Substitute.For<IFileCopierFactory>();
            _fileCopierFactory.CreateFileCopier(_unknownVerifier.FullSourcePath, _unknownVerifier.FullTargetPath)
                .Returns(_unknownVerifierCopier);
            _fileCopierFactory.CreateFileCopier(_misMatchedVerifier.FullSourcePath, _misMatchedVerifier.FullTargetPath)
                .Returns(_mismatchedVerifierCopier);
            _fileCopierFactory.CreateFileCopier(_missingTargetVerifier.FullSourcePath, _missingTargetVerifier.FullTargetPath)
                .Returns(_missingTargetVerifierCopier);

            _verifiers = new List<IHashVerifier>{_unknownVerifier, _matchedVerifier, _misMatchedVerifier, _missingTargetVerifier};
        }

        [Test]
        public void Synchronize_ExpectAllUnverifiedFilesToBeCopiedOver()
        {
            var synchronizer =
                new DirectorySyncher(_fileSystem, _pathUtility, _fileCopierFactory) {OpenLogInNotepad = false};
            synchronizer.Synchronize(_verifiers);
            
            Assert.IsTrue(_mismatchedVerifierCopier.PerformCopyWasCalled, "Expected mismatched verifier to result in a file copy");
            Assert.IsTrue(_missingTargetVerifierCopier.PerformCopyWasCalled, "Expected missing target verifier to result in a file copy");
            Assert.IsTrue(_unknownVerifierCopier.PerformCopyWasCalled, "Expected inability to generate hash to result in a file copy");
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