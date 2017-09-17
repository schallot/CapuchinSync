using System;
using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.DirectorySynchronization
{
    public class HashVerifier : Loggable, IHashVerifier
    {
        private VerificationStatus _status = VerificationStatus.TargetFileNotRead;
        private string _calculatedHash;
        private readonly IFileSystem _fileSystem;
        private readonly IHashUtility _hashUtility;
        private readonly IHashDictionaryEntry hashEntry;

        public IHashDictionaryEntry GetHashEntry()
        {
            return hashEntry;
        }

        public enum VerificationStatus
        {
            TargetFileMatchesHash,
            TargetFileDoesNotMatchHash,
            TargetFileDoesNotExist,
            TargetFileNotRead
        }

        public HashVerifier(IHashDictionaryEntry entry, string targetDirectory, 
            IFileSystem fileSystem, IPathUtility pathUtility,
            IHashUtility hashUtility)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }
            if (pathUtility == null)
            {
                throw new ArgumentNullException(nameof(pathUtility));
            }
            if (string.IsNullOrWhiteSpace(targetDirectory))
            {
                throw new ArgumentNullException(nameof(targetDirectory));
            }
            _hashUtility = hashUtility ?? throw new ArgumentNullException(nameof(hashUtility));
            Trace($"Creating {hashUtility.HashName} {nameof(HashVerifier)} for hash dictionary entry:<{entry}> with root source directory <{entry.RootDirectory}> and root target directory <{targetDirectory}>");
            hashEntry = entry;
            RootSourceDirectory = entry.RootDirectory;
            RootTargetDirectory = targetDirectory;
            _fileSystem = fileSystem;
            FullSourcePath = pathUtility.Combine(RootSourceDirectory, GetHashEntry().RelativePath);
            FullTargetPath = pathUtility.Combine(RootTargetDirectory, GetHashEntry().RelativePath);
        }

        public VerificationStatus Status 
        {
            get
            {
                if (_status == VerificationStatus.TargetFileNotRead)
                {
                    if (GetHashEntry().Hash == HashDictionaryEntry.UnknownHash)
                    {
                        _status = VerificationStatus.TargetFileDoesNotMatchHash;
                        // TODO: Examine file size, last write time to determine if we really need to copy?
                        Warn($"No hash was present for file {FullTargetPath}.  The file will be overwritten with source {FullSourcePath}.  When possible, try regenerating the hash dictionary file.");
                    }
                    else if (!_fileSystem.DoesFileExist(FullTargetPath))
                    {
                        _status = VerificationStatus.TargetFileDoesNotExist;
                        Info($"File at {FullTargetPath} does not exist, so it will have to be copied from source.");
                    }
                    else
                    {
                        try
                        {                            
                            if (CalculatedHash == GetHashEntry().Hash)
                            {
                                _status = VerificationStatus.TargetFileMatchesHash;
                                Debug($"File at {FullTargetPath} matches source at {FullSourcePath} with hash {CalculatedHash}, so no further action is needed.");
                            }
                            else
                            {
                                _status = VerificationStatus.TargetFileDoesNotMatchHash;
                                Info($"File at {FullTargetPath} has hash {CalculatedHash}, which does not match {GetHashEntry().Hash} of source {FullSourcePath}, so it will have to be copied from source.");
                            }
                        }
                        catch (Exception e)
                        {
                            Error($"Failed to verify hash of file at {FullTargetPath} - expected hash {GetHashEntry().Hash}", e);
                            _status = VerificationStatus.TargetFileNotRead;
                        }
                    }
                }
                return _status;
            }
        }

        public string CalculatedHash
        {
            get
            {
                if (string.IsNullOrEmpty(_calculatedHash))
                {
                    _calculatedHash = _hashUtility.GetHashFromFile(FullTargetPath);
                }
                return _calculatedHash;
            }
        }

        public string FullSourcePath { get; }
        public string FullTargetPath { get; }
        public string RootSourceDirectory { get; }
        public string RootTargetDirectory { get; }

    }
}
