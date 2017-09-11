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

        private IFileCopier _unknownVerifierCopier;
        private IFileCopier _mismatchedVerifierCopier;
        private IFileCopier _missingTargetVerifierCopier;

        private const string RootSourcePath = "C:\\source";
        private const string RootTargetPath = "D:\\target";

        [SetUp]
        public void SetUp()
        {
            _fileSystem = Substitute.For<IFileSystem>();
            _pathUtility = Substitute.For<IPathUtility>();
            _fileCopierFactory = Substitute.For<IFileCopierFactory>();

            _unknownVerifierCopier = Substitute.For<IFileCopier>();
            _mismatchedVerifierCopier = Substitute.For<IFileCopier>();
            _missingTargetVerifierCopier = Substitute.For<IFileCopier>();

            _unknownVerifier = Substitute.For<IHashVerifier>();
            _unknownVerifier.CalculatedHash.Returns("123");
            _unknownVerifier.FullSourcePath.Returns("unknownVerifierFullSourcePath");
            _unknownVerifier.FullTargetPath.Returns("unknownVerifierFullTargetPath");
            _unknownVerifier.RootSourceDirectory.Returns(RootSourcePath);
            _unknownVerifier.RootTargetDirectory.Returns(RootTargetPath);
            _unknownVerifier.Status.Returns(HashVerifier.VerificationStatus.TargetFileNotRead);

            _matchedVerifier = Substitute.For<IHashVerifier>();
            _matchedVerifier.CalculatedHash.Returns("456");
            _matchedVerifier.FullSourcePath.Returns("matchedVerifierFullSourcePath");
            _matchedVerifier.FullTargetPath.Returns("matchedVerifierFullTargetPath");
            _matchedVerifier.RootSourceDirectory.Returns(RootSourcePath);
            _matchedVerifier.RootTargetDirectory.Returns(RootTargetPath);
            _matchedVerifier.Status.Returns(HashVerifier.VerificationStatus.TargetFileMatchesHash);

            _misMatchedVerifier = Substitute.For<IHashVerifier>();
            _misMatchedVerifier.CalculatedHash.Returns("789");
            _misMatchedVerifier.FullSourcePath.Returns("misMatchedVerifierFullSourcePath");
            _misMatchedVerifier.FullTargetPath.Returns("misMatchedVerifierFullTargetPath");
            _misMatchedVerifier.RootSourceDirectory.Returns(RootSourcePath);
            _misMatchedVerifier.RootTargetDirectory.Returns(RootTargetPath);
            _misMatchedVerifier.Status.Returns(HashVerifier.VerificationStatus.TargetFileDoesNotMatchHash);

            _missingTargetVerifier = Substitute.For<IHashVerifier>();
            _missingTargetVerifier.CalculatedHash.Returns("0AB");
            _missingTargetVerifier.FullSourcePath.Returns("missingTargetVerifierFullSourcePath");
            _missingTargetVerifier.FullTargetPath.Returns("missingTargetVerifierFullTargetPath");
            _missingTargetVerifier.RootSourceDirectory.Returns(RootSourcePath);
            _missingTargetVerifier.RootTargetDirectory.Returns(RootTargetPath);
            _misMatchedVerifier.Status.Returns(HashVerifier.VerificationStatus.TargetFileDoesNotExist);

            _fileCopierFactory = Substitute.For<IFileCopierFactory>();
            _fileCopierFactory.CreateFileCopier(_unknownVerifier.FullSourcePath, _unknownVerifier.FullTargetPath)
                .Returns(_unknownVerifierCopier);
            _fileCopierFactory.CreateFileCopier(_misMatchedVerifier.FullSourcePath, _misMatchedVerifier.FullTargetPath)
                .Returns(_mismatchedVerifierCopier);
            _fileCopierFactory.CreateFileCopier(_missingTargetVerifier.FullSourcePath, _missingTargetVerifier.FullTargetPath)
                .Returns(_missingTargetVerifierCopier);

            _unknownVerifierCopier.SuccesfullyCopied.Returns(true);
            _mismatchedVerifierCopier.SuccesfullyCopied.Returns(true);
            _missingTargetVerifierCopier.SuccesfullyCopied.Returns(true);

            _verifiers = new List<IHashVerifier>{_unknownVerifier, _matchedVerifier, _misMatchedVerifier, _missingTargetVerifier};
        }

        [Test]
        public void SynchronizeTest()
        {
            var synchronizer =
                new DirectorySyncher(_fileSystem, _pathUtility, _fileCopierFactory) {OpenLogInNotepad = false};
            synchronizer.Synchronize(_verifiers);
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